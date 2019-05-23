# Silk

## Using the Library

The Silk library is designed to be very simple to use. The first step is to create an instance of the `Compiler` class.

The next step is the critical part. Use the `RegisterFunction` and `RegisterVariable` methods to add functions and variables, which will be available to the source code. This will give the language the ability to perform the tasks you choose specific to the domain of your application.

Next, call the `Compile` method. If it returns `false`, the compile failed and you can use the `Errors` property to access the compile errors. Otherwise, the `Compile` method returns an instance of the `CompiledProgram` class.

This is demonstrated in the following example.

```cs
Compiler compiler = new Compiler();
compiler.EnableLineNumbers = true;

// Register intrinsic functions
compiler.RegisterFunction("Print", 0, Function.NoParameterLimit);
compiler.RegisterFunction("Color", 1, 2);
compiler.RegisterFunction("ClearScreen", 0, 0);
compiler.RegisterFunction("ReadKey", 0, 0);

// Register intrinsic variables
foreach (var color in Enum.GetValues(typeof(ConsoleColor)))
  compiler.RegisterVariable(color.ToString(), new Variable((int)color));

if (compiler.Compile(path, out CompiledProgram program))
{
  Console.WriteLine("Compile succeeded.");
}
else
{
  Console.WriteLine("Compile failed.");
  Console.WriteLine();
  foreach (Error error in compiler.Errors)
    Console.WriteLine(error.ToString());
}
```

The `CompiledProgram` object contains the compiled code. You can use this class' `Save` and `Load` methods to save a compiled program to a file, and load a compiled program from a file. This allows you to load a previously compiled program and run it without needing to compile it each time.

To run a `CompiledProgram`, create an instance of the `Runtime` class and pass the `CompiledProgram` to the `Execute` method. The `Runtime` class has three methods: `Begin`, `Function` and `End`.

The `Begin` event is called when the program starts to run. The `BeginEventArgs` argument contains a property called `UserData`. You can use this property to store any contextual data in your application and this same object will be passed to the other `Runtime` events.

The `Function` event is called when the program executes a call that you registered with `Compiler.RegisterFunction`. The `FunctionEventArgs` includes a Parameters` property, which is an array of arguments that were passed to the function. And it includes a `ReturnValue` property, which specifies the function's return value.

This is demonstrated by the following example.

```cs
private void RunProgram(CompiledProgram program)
{
  Runtime runtime = new Runtime();
  runtime.Begin += Runtime_Begin;
  runtime.Function += Runtime_Function;
  runtime.End += Runtime_End;

  Variable result = runtime.Execute(program);

  Console.WriteLine();
  Console.WriteLine($"Program ran successfully with exit code {result}.");
}

private static void Runtime_Begin(object sender, BeginEventArgs e)
{
  e.UserData = this;
}

private static void Runtime_Function(object sender, FunctionEventArgs e)
{
  switch (e.Name)
  {
    case "Print":
      Console.WriteLine(string.Join('\t', e.Parameters.Select(p => p.ToString())));
      break;
    case "Color":
      Debug.Assert(e.Parameters.Length >= 1);
      Debug.Assert(e.Parameters.Length <= 2);
      if (e.Parameters.Length >= 1)
        Console.ForegroundColor = (ConsoleColor)e.Parameters[0].ToInteger();
      if (e.Parameters.Length >= 2)
        Console.BackgroundColor = (ConsoleColor)e.Parameters[1].ToInteger();
      break;
    case "ClearScreen":
      Console.Clear();
      break;
    case "ReadKey":
      e.ReturnValue.SetValue(Console.ReadKey().KeyChar);
      break;
    default:
      Debug.Assert(false);
      break;
  }
}

private static void Runtime_End(object sender, EndEventArgs e)
{
}
```


## Introduction

The Simple Interpreted Language Kit (SILK) is a .NET class library that makes it easy to add scripting and automation to any .NET application.

The language is designed to be minimal and relatively easy to use. It's very loose with data types: you can mix integers, floating-point and string values in expressions. If possible, the language will simply do the best it can with expressions that don't make sense rather than raise an error or exception.

The language is also streamlined in that it has a small set of internal functions. For example, there are no functions to display information to, or get input from the user. Rather, the library makes no assumptions about the environment or whether the app will be a console or windowed application. It is up to the host application to provide functions for needed tasks, which the library makes very easy to do.

## Functions

All statements must appear inside a function. A function is declared by the function name, followed by
open and closing parentheses. Optionally, identifier names can appear within the parentheses. Multiple
identifiers must be separated by commas. These identifiers specify the function's parameters, which will
take on the value of arguments passed to the function.

The parentheses must be followed by open and closing curly braces. The statements for this function
must appear within these curly braces. Functions cannot be nested (defined inside of another function).

All programs must define a function called main. When the program is executed, this function will be called
by the runtime. And so the main function is where execution begins. All other functions in the code will only
be called if the code explicitly calls them.

The following example creates an empty function called main.

```cs
main()
{
}
```

Execution will return to the previous function when the closing curly brace is reached. In addition, the
return keyword can be used to return at any point within the function. The return keyword can optionally
be followed by any valid expression. In this case, the expression will be evaluated and the result will
be returned by the function.

The following example declares a function named main, and another function named double. The main function
calls double, which takes a single argument and multiplies it by 2, and assigns the result to a variable
named i. The result is that the variable i would have the value 4.

```cs
main()
{
    i = double(2)
}

double(value)
{
    return value * 2
}
```

## Variables

A variable is an identifer that represents a value. Variables can be declared a couple of ways.

Within a function, you can declare a variable simply by assigning a value to an identifier. The
following example declares a variable named i and gives it a value of 5.

```cs
i = 5
```

Optionally, you can preceed the identifier with the var keyword. When you use the var keyword,
assigning a value to the variable is optional. When you don't assign a value, the variable will
have the default value of 0. So the following two examples would produce the same result.

Example 1:

```cs
var i
```

Example 2:

```cs
i = 0
```

The variables described above are local to the function where they are declared. This means that
if you declare a variable with the same name in two different functions, they will be independent
and contain completely separate values.

If you want to share a variable between functions, you can declare a global variable. All global
variables must be declared before the first function in your source code. This is the only case
where code can appear outside of a function. When declaring a global variable, the var keyword
is required. It is optional to initialize the variable with a value.

The following example declares two global variables. The first time doSomething is called, it
assigns the value 5 to i. The second time it is called, it assigns the value 10 to i.

```cs
var i       // Initialized with default value of 0
var j = 5   // Initialized to 5

main()
{
    doSomething
    j = 10
    doSomething
}

doSomething()
{
    i = j
}
```

## Lists (Arrays)

Silk also supports lists. There are two ways to create a list. The first way is using square
brackets to specify the size of the list.

```cs
a = [10]
```

The example above creates a list with 10 variables. It does not initialize the values of those
variables, and so they all have default values (the default value is zero).

The second approach uses curly braces to initialize the value of each variable in the list.

```cs
a = { "abc", 27, "def", 50, 1.5 }
```

The example above creates a list with five variables and initializes the value of each variable.
As you can see, each variable can be initialized to any type of value, including other lists.

You can use square brackets to access variables in the list. The number within the square
brackets specifies the 1-based list index. So the following example would assign a value to the
first variable in the list.

a[1] = 25

And the next example reads the variable at the fifth position within the list.

i = a[5]

Global lists can be created using the VAR keyword before any functions are defined. The following
example creates a global list with ten variables, two of the variables are also lists.

```cs
var global = { 1, 2, 3, 4, 5, 6, 7, "abc", [10], { 10, 20, 30 } }
```

As when intializing global variables before any functions are defined, the variable values must
be literals (no variables or function return values). If a list is initialized within a function,
default values can include expressions, variables and function return values.

## Code

No more than one statement per line.

## Comments

The SILK language supports C-style comments. There are two forms of comments.

Line Comment
A line comment is signifies by two forward slashes (`//`). The forward slashes and everything that follows on
the same line will be considered as a comment and ignored by the compiler.

Multiline Comment
A multiline comment starts with a forward slash and asterisk (`/*`) and ends with an asterisk and forward
slash (`*/`). These delimiters and anything that appears between them will be considered a comment and ignored
by the compiler.




Intrinsic functions
-> Number of parameters passed restricted per settings at compile time






## Internal Functions

### Abs(*value*)

Returns the absolute value of *value*.

### Acos(*value*)

Returns the angle whose cosine is equal to *value*.

### Asc(*s*)

Returns the ASCII/Unicode value of the first character in the string *s*.

### Atn(*value*)

Returns an angle whose tangent is equal to *value*.

### Avg(*value*, ...)

Returns the average of the given arguments. Any number of arguments can be provided. In addition, any argument can be a list. In this case, each item in the list will be averaged.

### Bin(*value*)

Converts the given value to a binary (base 2) string.

### Chr(*value*)

Creates a string with one character with the specified ASCII/Unicode value.

### Cos(*value*)

Returns the cosine of *value*.

### Date()

Returns a string with the current date.

### Environ(*name*)

Returns the value of the specified environment variable.

### Exp(*value*)

Returns `e` raised to the specified power.

### Hex(*value*)

Returns a hexedecimal string equal to the given value.

### InStr(*s1*, *s2*[, *start*])

Returns the 1-based position where the string *s2* appears within the the string *s1*. The optional *start* argument specifies the 1-based position to begin the search.

### Int()

Converts the given value to an integer, truncating any fractional portion.

### IsList(*a*)

Returns true if the variable *a* contains a list.

### Left(*s*, *count*)

Returns a string with the left-most *count* characters from *s*.

### Len(*value*)

If *value* is a list, this function returns the number of items in the list. Otherwise, it returns the number of characters in *value*, converted to a string if needed.

### Max(*value*, ...)

Returns the maximum value of the given arguments. Any number of arguments can be provided. In addition, any argument can be a list. In this case, the maximum value of each item in the list is returned.

### Mid(*s*, *start*[, *count*])

Returns a section of the string given as the first argument. The *start* specifies the 1-based position where the string should be extracted. If *count* is provided, it specifies the maximum number of characters to return.

### Min(*value*, ...)

Returns the minimum value of the given arguments. Any number of arguments can be provided. In addition, any argument can be a list. In this case, the minimum value of each item in the list is returned.

### Oct(*value*)

Converts the given *value* to an octal string.

### Right(*s*, *count*)

Returns a string with the right-most *count* characters from *s*.

### Round(*value*)

Rounds the given value to the nearest integer.

### Sin(*value*)

Returns the sign of the angle specified by *value*.

### Sqr(*value*)

Returns the square root of the given argument.

### String(*value*, *count*)

Returns a string with *value* repeated the number of times specified by *count*.

If *value* is a stringr value, then that string is repeated. Otherwise, the string repeated will contain one character with the specified ASCII/Unicode value specified by *value*

### Tan(*value*)

Returns the tanget of the angle specified by *value*.

### Time()

Returns a string with the current time.

## Internal Variables

### E

Represents the natural logarithmic base, specified by the constant *e*.

### Pi

Represents the ratio of the circumference of a circle to its diameter, specified by the constant *&pi;*.
