// Copyright (c) 2019-2022 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SoftCircuits.HtmlMonkey
{
    public class TextParser
    {
        private int InternalIndex;

        /// <summary>
        /// Represents an invalid character. This character is returned when attempting to read
        /// a character at an invalid position. The character value is <c>'\0'</c>.
        /// </summary>
        public const char NullChar = '\0';

        /// <summary>
        /// Returns the text currently being parsed.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Constructs a new <see cref="ParsingHelper"></see> instance. Sets the text to be parsed
        /// and sets the current position to the start of that text.
        /// </summary>
        /// <param name="text">The text to be parsed. Can be <c>null</c>.</param>
        /// all methods that use regular expressions.</param>
        public TextParser(string? text)
        {
            Reset(text);
        }

        /// <summary>
        /// Sets the text to be parsed and sets the current position to the start of that text.
        /// </summary>
        /// <param name="text">The text to be parsed. Can be <c>null</c>.</param>
#if !NETSTANDARD
        [MemberNotNull(nameof(Text))]
#endif
        public void Reset(string? text)
        {
            Text = text ?? string.Empty;
            InternalIndex = 0;
        }

        /// <summary>
        /// Gets or sets the current position within the text being parsed. Safely
        /// handles attempts to set to an invalid position.
        /// </summary>
        public int Index
        {
            get => InternalIndex;
            set
            {
                InternalIndex = value;
                if (InternalIndex < 0)
                    InternalIndex = 0;
                else if (InternalIndex > Text.Length)
                    InternalIndex = Text.Length;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the current position is at the end of the text being parsed.
        /// Otherwise, false.
        /// </summary>
        public bool EndOfText => (InternalIndex >= Text.Length);

        /// <summary>
        /// Returns the character at the current position, or <see cref="NullChar"/>
        /// if the current position was at the end of the text being parsed.
        /// </summary>
        /// <returns>The character at the current position.</returns>
        public char Peek()
        {
            Debug.Assert(InternalIndex >= 0 && InternalIndex <= Text.Length);
            return (InternalIndex < Text.Length) ? Text[InternalIndex] : NullChar;
        }

        /// <summary>
        /// Returns the character at the specified number of characters ahead of the
        /// current position, or <see cref="NullChar"></see> if the specified position
        /// is not valid. Does not change the current position.
        /// </summary>
        /// <param name="count">Specifies the position of the character to read as the number
        /// of characters ahead of the current position. May be a negative number.</param>
        /// <returns>The character at the specified position.</returns>
        public char Peek(int count)
        {
            int index = (InternalIndex + count);
            return (index >= 0 && index < Text.Length) ? Text[index] : NullChar;
        }

        /// <summary>
        /// Returns the character at the current position and increments the current position.
        /// Returns <see cref="NullChar"/> if the current position was at the end of the text
        /// being parsed.
        /// </summary>
        /// <returns>The character at the current position.</returns>
        public char Get()
        {
            Debug.Assert(InternalIndex >= 0 && InternalIndex <= Text.Length);
            if (InternalIndex < Text.Length)
                return Text[InternalIndex++];
            return NullChar;
        }

        /// <summary>
        /// Moves the current position ahead one character.
        /// </summary>
        public void Next()
        {
            Debug.Assert(InternalIndex >= 0 && InternalIndex <= Text.Length);
            if (InternalIndex < Text.Length)
                InternalIndex++;
        }

        /// <summary>
        /// Moves the current position to the next character that causes <paramref name="predicate"/>
        /// to return <c>false</c>.
        /// </summary>
        /// <param name="predicate">Function to return test each character and return <c>true</c>
        /// for each character that should be skipped.</param>
        public void SkipWhile(Func<char, bool> predicate)
        {
            Debug.Assert(InternalIndex >= 0 && InternalIndex <= Text.Length);
            while (InternalIndex < Text.Length && predicate(Text[InternalIndex]))
                InternalIndex++;
        }

        /// <summary>
        /// Moves the current position to the next character that is not a whitespace character.
        /// </summary>
        public void SkipWhiteSpace() => SkipWhile(char.IsWhiteSpace);

        /// <summary>
        /// Moves the current position to the next character that is one of the specified characters
        /// and returns <c>true</c> if a match was found. If none of the specified characters are
        /// found, this method moves the current position to the end of the text being parsed and
        /// returns <c>false</c>.
        /// </summary>
        /// <param name="chars">Characters to skip to.</param>
        /// <returns>True if any of the specified characters were found. Otherwise, false.</returns>
        public bool SkipTo(params char[] chars)
        {
            InternalIndex = Text.IndexOfAny(chars, InternalIndex);
            if (InternalIndex >= 0)
                return true;
            InternalIndex = Text.Length;
            return false;
        }

        /// <summary>
        /// Moves the current position to the next occurrence of the specified string and returns
        /// <c>true</c> if a match was found. If the specified string is not found, this method
        /// moves the current position to the end of the text being parsed and returns <c>false</c>.
        /// </summary>
        /// <param name="s">String to skip to.</param>
        /// <param name="includeToken">If <c>true</c> and a match is found, the matching string is
        /// also skipped.</param>
        /// <returns>True if the specified string was found. Otherwise, false.</returns>
        public bool SkipTo(string s, bool includeToken = false)
        {
            InternalIndex = Text.IndexOf(s, InternalIndex);
            if (InternalIndex >= 0)
            {
                if (includeToken)
                    InternalIndex += s.Length;
                return true;
            }
            InternalIndex = Text.Length;
            return false;
        }

        /// <summary>
        /// Moves the current position to the next occurrence of the specified string and returns
        /// <c>true</c> if a match was found. If the specified string is not found, this method
        /// moves the current position to the end of the text being parsed and returns <c>false</c>.
        /// </summary>
        /// <param name="s">String to skip to.</param>
        /// <param name="comparison">One of the enumeration values that specifies the rules for
        /// search.</param>
        /// <param name="includeToken">If <c>true</c> and a match is found, the matching text is
        /// also skipped.</param>
        /// <returns>True if the specified string was found. Otherwise, false.</returns>
        public bool SkipTo(string s, StringComparison comparison, bool includeToken = false)
        {
            InternalIndex = Text.IndexOf(s, InternalIndex, comparison);
            if (InternalIndex >= 0)
            {
                if (includeToken)
                    InternalIndex += s.Length;
                return true;
            }
            InternalIndex = Text.Length;
            return false;
        }

        /// <summary>
        /// Parses a single character and increments the current position. Returns an empty string
        /// if the current position was at the end of the text being parsed.
        /// </summary>
        /// <returns>A string that contains the parsed character, or an empty string if the current
        /// position was at the end of the text being parsed.</returns>
        public string ParseCharacter()
        {
            Debug.Assert(InternalIndex >= 0 && InternalIndex <= Text.Length);
            if (InternalIndex < Text.Length)
                return Text[InternalIndex++].ToString();
            return string.Empty;
        }

        /// <summary>
        /// Parses characters until the next character for which <paramref name="predicate"/>
        /// returns <c>false</c>, and returns the parsed characters. Can return an empty string.
        /// </summary>
        /// <param name="predicate">Function to test each character. Should return <c>true</c>
        /// for each character that should be parsed.</param>
        /// <returns>A string with the parsed characters.</returns>
        public string ParseWhile(Func<char, bool> predicate)
        {
            int start = InternalIndex;
            SkipWhile(predicate);
            return Extract(start, InternalIndex);
        }

        /// <summary>
        /// Parses quoted text. The character at the current position is assumed to be the starting quote
        /// character. This method parses text up until the matching end quote character. Returns the parsed
        /// text without the quotes and sets the current position to the character following the
        /// end quote. If the text contains two quote characters together, the pair is handled as a
        /// single quote literal and not the end of the quoted text.
        /// </summary>
        /// <returns>Returns the text within the quotes.</returns>
        public string ParseQuotedText()
        {
            StringBuilder builder = new();

            // Get and skip quote character
            char quote = Get();

            // Parse quoted text
            while (!EndOfText)
            {
                // Parse to next quote
                builder.Append(ParseTo(quote));
                // Skip quote
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
        /// Parses characters until the next occurrence of any one of the specified characters and
        /// returns a string with the parsed characters. If none of the specified characters are found,
        /// this method parses all character up to the end of the text being parsed. Can return an empty
        /// string.
        /// </summary>
        /// <param name="chars">The characters that cause parsing to end.</param>
        /// <returns>A string with the parsed characters.</returns>
        public string ParseTo(params char[] chars)
        {
            int start = InternalIndex;
            SkipTo(chars);
            return Extract(start, InternalIndex);
        }

        /// <summary>
        /// Parses characters until the next occurrence of the specified string and returns a
        /// string with the parsed characters. If the specified string is not found, this method parses
        /// all character to the end of the text being parsed. Can return an empty string.
        /// </summary>
        /// <param name="s">Text that causes parsing to end.</param>
        /// <param name="comparison">One of the enumeration values that specifies the rules for
        /// comparing the specified string.</param>
        /// <param name="includeToken">If <c>true</c> and a match is found, the matching text is
        /// also parsed.</param>
        /// <returns>A string with the parsed characters.</returns>
        public string ParseTo(string s, StringComparison comparison, bool includeToken = false)
        {
            int start = InternalIndex;
            SkipTo(s, comparison, includeToken);
            return Extract(start, InternalIndex);
        }

        /// <summary>
        /// Returns <c>true</c> if the given string matches the characters at the current position, or
        /// <c>false</c> otherwise.
        /// </summary>
        /// <param name="s">String to compare.</param>
        /// <param name="comparison">One of the enumeration values that specifies the rules to use in the
        /// comparison.</param>
        /// <returns>Returns <c>true</c> if the given string matches the characters at the current position,
        /// of <c>false</c> otherwise.</returns>
        public bool MatchesCurrentPosition(string? s, StringComparison comparison) => s != null &&
            s.Length != 0 &&
            string.Compare(Text, InternalIndex, s, 0, s.Length, comparison) == 0;

        /// <summary>
        /// Extracts a substring from the specified range of the text being parsed.
        /// </summary>
        /// <param name="start">0-based position of first character to be extracted.</param>
        /// <param name="end">0-based position of the character that follows the last
        /// character to be extracted.</param>
        /// <returns>Returns the extracted string.</returns>
        public string Extract(int start, int end)
        {
            if (start < 0 || start > Text.Length)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (end < start || end > Text.Length)
                throw new ArgumentOutOfRangeException(nameof(end));
#if NETSTANDARD
            return Text.Substring(start, end - start);
#else
            return Text[start..end];
#endif
        }
    }
}
