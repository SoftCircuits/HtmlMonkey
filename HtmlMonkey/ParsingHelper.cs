// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Text;

namespace SoftCircuits.HtmlMonkey
{
    /// <summary>
    /// Low-level text parsing helper class.
    /// </summary>
    public class ParsingHelper
    {
        /// <summary>
        /// Represents a non-valid character. This character is returned when a valid character
        /// is not available, such as when returning a character beyond the end of the text.
        /// The character is represented as <c>'\0'</c>.
        /// </summary>
        public const char NullChar = '\0';

        /// <summary>
        /// Returns the current text being parsed.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Returns the current position within the text being parsed.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Constructs a TextParse instance.
        /// </summary>
        /// <param name="text">The text to be parsed.</param>
        public ParsingHelper(string text)
        {
            Reset(text);
        }

        /// <summary>
        /// Sets the text to be parsed and resets the current position to the start of that text.
        /// </summary>
        /// <param name="text">The text to be parsed.</param>
        public void Reset(string text)
        {
            Text = text ?? string.Empty;
            Index = 0;
        }

        /// <summary>
        /// Indicates if the current position is at the end of the text being parsed.
        /// </summary>
        public bool EndOfText => (Index >= Text.Length);

        /// <summary>
        /// Returns the number of characters not yet parsed. This is equal to the length of the
        /// text being parsed minus the current position within that text.
        /// </summary>
        public int Remaining => (Text.Length - Index);

        /// <summary>
        /// Returns the character at the current position, or <see cref="NullChar"></see> if
        /// we're at the end of the text being parsed.
        /// </summary>
        /// <returns>The character at the current position.</returns>
        public char Peek() => Peek(0);

        /// <summary>
        /// Returns the character at the specified number of characters beyond the current
        /// position, or <see cref="NullChar"></see> if the specified position is out of
        /// bounds of the text being parsed.
        /// </summary>
        /// <param name="count">The number of characters beyond the current position.</param>
        /// <returns>The character at the specified position.</returns>
        public char Peek(int count)
        {
            int pos = (Index + count);
            return (pos >= 0 && pos < Text.Length) ? Text[pos] : NullChar;
        }

        /// <summary>
        /// Moves the current position ahead one character.
        /// </summary>
        public void Next()
        {
            if (Index < Text.Length)
                Index++;
        }

        /// <summary>
        /// Moves the current position ahead the specified number of characters.
        /// </summary>
        /// <param name="count">The number of characters to move ahead. Use negative numbers
        /// to move back.</param>
        public void Next(int count)
        {
            int index = Index + count;
            if (index < 0)
                Index = 0;
            else if (index > Text.Length)
                Index = Text.Length;
            else
                Index = index;
        }

        /// <summary>
        /// Moves the current position to the next occurrence of the specified string and returns
        /// <c>true</c> if successful. If the specified string is not found, this method moves the
        /// current position to the end of the input text and returns <c>false</c>.
        /// </summary>
        /// <param name="s">String to find.</param>
        /// <returns>Returns a Boolean value that indicates if any of the specified characters
        /// were found.</returns>
        public bool SkipTo(string s, StringComparison comparison = StringComparison.Ordinal)
        {
            int index = Text.IndexOf(s, Index, comparison);
            if (index >= 0)
            {
                Index = index;
                return true;
            }
            Index = Text.Length;
            return false;
        }

        /// <summary>
        /// Moves to the next occurrence of any one of the specified characters and returns
        /// <c>true</c> if successful. If none of the specified characters are found, this method
        /// moves the current position to the end of the input text and returns <c>false</c>.
        /// </summary>
        /// <param name="chars">Characters to skip to.</param>
        /// <returns>Returns a Boolean value that indicates if any of the specified characters
        /// were found.</returns>
        public bool SkipTo(params char[] chars)
        {
            int index = Text.IndexOfAny(chars, Index);
            if (index >= 0)
            {
                Index = index;
                return true;
            }
            Index = Text.Length;
            return false;
        }

        /// <summary>
        /// Moves the current position to the next character that is not whitespace.
        /// </summary>
        public void SkipWhiteSpace()
        {
            SkipWhile(char.IsWhiteSpace);
        }

        /// <summary>
        /// Moves the current position to the next character that causes <paramref name="predicate"/>
        /// to return false.
        /// </summary>
        /// <param name="predicate">Function to test each character.</param>
        public void SkipWhile(Func<char, bool> predicate)
        {
            while (!EndOfText && predicate(Peek()))
                Next();
        }

        /// <summary>
        /// Parses characters until the next character that causes <paramref name="predicate"/> to
        /// return false, and then returns the characters spanned. Can return an empty string.
        /// </summary>
        /// <param name="predicate">Function to test each character.</param>
        public string ParseWhile(Func<char, bool> predicate)
        {
            int start = Index;
            while (!EndOfText && predicate(Peek()))
                Next();
            return Extract(start, Index);
        }

        /// <summary>
        /// Parses a quoted string. Interprets the character at the starting position as the quote
        /// character. Two quote characters together within the string are interpreted as a single
        /// quote literal and not the end of the string. Returns the text within the quotes and
        /// sets the current position to the first character after the ending quote character.
        /// </summary>
        public string ParseQuotedText()
        {
            // Get quote character
            char quote = Peek();
            // Skip quote
            Next();
            // Parse quoted text
            StringBuilder builder = new StringBuilder();
            while (!EndOfText)
            {
                int start = Index;
                // Move to next quote
                SkipTo(quote);
                // Capture quoted text
                builder.Append(Extract(start, Index));
                // Skip over quote
                Next();
                // Two consecutive quotes treated as quote literal
                if (Peek() == quote)
                {
                    builder.Append(quote);
                    Next();
                }
                else break; // Done if single closing quote or end of text
            }
            return builder.ToString();
        }

        /// <summary>
        /// Compares the given string to text at the current position.
        /// </summary>
        /// <param name="s">String to compare.</param>
        /// <param name="comparison">Type of string comparison to use.</param>
        /// <returns>Returns <c>true</c> if the given string matches the text at the current position.
        /// Returns false otherwise.</returns>
        public bool MatchesCurrentPosition(string s, StringComparison comparison = StringComparison.Ordinal)
        {
            // TODO: Rewrite to use ReadOnlySpan<char> when available
            if (s == null || s.Length == 0 || Remaining < s.Length)
                return false;
            return s.Equals(Text.Substring(Index, s.Length), comparison);
        }

        /// <summary>
        /// Extracts a substring from the specified range of the text being parsed.
        /// </summary>
        /// <param name="start">0-based position of first character to be extracted.</param>
        /// <param name="end">0-based position of the character that follows the last
        /// character to be extracted.</param>
        /// <returns>Returns the extracted string.</returns>
        public string Extract(int start, int end) => Text.Substring(start, end - start);

        #region Operator overloads

        public static implicit operator int(ParsingHelper helper) => helper.Index;

        public static ParsingHelper operator ++(ParsingHelper helper)
        {
            helper.Next(1);
            return helper;
        }

        public static ParsingHelper operator --(ParsingHelper helper)
        {
            helper.Next(-1);
            return helper;
        }

        public static ParsingHelper operator +(ParsingHelper helper, int count)
        {
            helper.Next(count);
            return helper;
        }

        public static ParsingHelper operator -(ParsingHelper helper, int count)
        {
            helper.Next(-count);
            return helper;
        }

        #endregion

    }
}
