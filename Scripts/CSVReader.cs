using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class CSVReader
{
    // flag for processing a CSV
    private enum ParsingMode { None /*default*/, OutQuote, /*processing a character which is out of quotes*/
        InQuote // processing a character which is in quotes
    }
    //I'll just pass in the name of the .csv file containing the relevant dialogue for this trigger
        //This function will return a 2D array, which I will then pass into my Dialogue class as my list of lines
        //Thus, skipping past the now redundant DialogueLine and DialogueCharacter classes - I'm sick of doing them manually and I haven't even finished the lore yet
    public List<List<string>> ParseCSV(string csvFileName){
        //Ideally, Application.dataPath should work for all devices
            //After all, kinda want to keep the lore folder close to my game
            //Accessible next to my Assets folder in the Editor and next to my executable in release
        string foundFile = Directory.GetParent(Application.dataPath).ToString() + new string($"\\Lore\\{csvFileName}");
        string src = File.ReadAllText(@foundFile); //should probably find a way to either insert filename in directory text or search for it
        var rows = new List<List<string>>();
        var cols = new List<string>();
        var buffer = new StringBuilder();

        ParsingMode mode = ParsingMode.OutQuote;
        bool requireTrimLineHead = false;
        var isBlank = new Regex(@"\s"); //This is a whitespace in regular expression - we're gonna use it to find other whitespaces

        int len = src.Length;
        for (int i = 0; i < len; ++i) {
            char c = src[i];
            // remove whilespace at beginning of line
            if (requireTrimLineHead)
            {
                if (isBlank.IsMatch(c.ToString())) continue;
                requireTrimLineHead = false;
            }

            // finalize when c is the last character
            if ((i + 1) == len)
            {
                // final char
                switch (mode)
                {
                    case ParsingMode.InQuote:
                        // if close quote is missing
                        if (c != '"') buffer.Append(c);
                        cols.Add(buffer.ToString());
                        rows.Add(cols);
                        return rows;

                    case ParsingMode.OutQuote:
                        if (c == '|')
                        {
                            // if the final character is comma, add an empty cell
                            cols.Add(buffer.ToString());
                            cols.Add(string.Empty);
                            rows.Add(cols);
                            return rows;
                        }
                        if (cols.Count == 0) {// if the final line is empty, ignore it. 
                            if (string.Empty.Equals(c.ToString().Trim())) return rows; }
                        buffer.Append(c);
                        cols.Add(buffer.ToString());
                        rows.Add(cols);
                        return rows;
                }
            }
            char n = src[i + 1]; // the next character
            switch (mode){
                case ParsingMode.OutQuote:
                    // out quote
                    if (c == '"'){
                        // to in-quote
                        mode = ParsingMode.InQuote;
                        continue;
                    }
                    else if (c == '|'){
                        // next cell
                        cols.Add(buffer.ToString());
                        buffer.Remove(0, buffer.Length);
                    }
                    else if (c == '\r' && n == '\n'){
                        // new line(CR+LF) - '\r' is the newline character for Mac specifically
                        cols.Add(buffer.ToString());
                        rows.Add(cols);
                        cols = new List<string>();
                        buffer.Remove(0, buffer.Length);
                        ++i; // skip next code
                        requireTrimLineHead = true;

                    }
                    else if (c == '\n' || c == '\r'){
                        // new line
                        cols.Add(buffer.ToString());
                        rows.Add(cols);
                        cols = new List<string>();
                        buffer.Remove(0, buffer.Length);
                        requireTrimLineHead = true;

                    }
                    else buffer.Append(c); // get one char
                    break;

                case ParsingMode.InQuote:
                    // in quote
                    if (c == '"' && n != '"') mode = ParsingMode.OutQuote;
                    else if (c == '"' && n == '"'){
                        // get "
                        buffer.Append('"');
                        ++i;
                    }
                    else buffer.Append(c); // get one char
                    break;
            }
        }
        return rows;
    }
}