using System;
using System.Collections.Generic;
using System.IO;

namespace Lab1LFPC
{
    class Node
    {
        public string TerminalValue { get; set; }

        public string NextState { get; set; }

    }
    class Program
    {
        static readonly string textFile = @"C:\Users\colea\Desktop\Lab1LFPC\Lab1LFPC\Varianta9.txt";
        public string[] Vn;
        public string[] Vt;
        public Dictionary<string, List<Node>> P = new Dictionary<string, List<Node>>();

        //function to convert from grammar to FA
        public void Parse()
        {
            using (StreamReader file = new StreamReader(textFile))
            {
                int counter = 0;
                string ln;

                while ((ln = file.ReadLine()) != null)
                {
                    counter++;
                    switch (counter)
                    {
                        case 1:
                            Vn = ln.Split(',');
                            break;
                        case 2:
                            Vt = ln.Split(',');
                            break;
                        case 3:
                            var transformations = ln.Split(',');
                            foreach (var transition in transformations)
                            {
                                string[] states;
                                if (transition.Length == 3)
                                {
                                    states = transition.Split('-');
                                    if (!P.ContainsKey(states[0]))
                                        P.Add(states[0], new List<Node>());
                                    P[states[0]].Add(new Node
                                    {
                                        NextState = "",
                                        TerminalValue = states[1][0].ToString()
                                    });
                                }
                                else
                                {

                                    states = transition.Split('-');
                                    if (!P.ContainsKey(states[0]))
                                        P.Add(states[0], new List<Node>());
                                    P[states[0]].Add(new Node
                                    {
                                        NextState = states[1][1].ToString(),
                                        TerminalValue = states[1][0].ToString()
                                    });
                                }
                            }
                            break;

                    }
                    Console.WriteLine(ln);
                }
                file.Close();
            }
        }

        //function to check next node if is valid
        public bool CheckLetter(string key, int index, string word, string path)
        {
            if (index >= word.Length) return false;
            var valid = false;
            foreach (var node in P[key])
            {
                if (word[index].ToString() == node.TerminalValue)
                {
                    if (index == word.Length - 1 && node.NextState == "")
                    {
                        Console.WriteLine($"Path:{path}" + key);
                        return true;
                    }
                    if (index < word.Length - 1 && node.NextState == "") valid = false;
                    else
                    {
                        valid = CheckLetter(node.NextState, ++index, word, path + key + "->");
                        if (valid) return true;
                    }
                };
            }
            return valid;
        }

        //function to generate words from gramar
        public void Generate(string key, string word)
        {
            if (word.Length <= Vn.Length + 4)
            {
                foreach (var node in P[key])
                {
                    if (node.NextState != "")
                        Generate(node.NextState, word + node.TerminalValue);
                    else Console.WriteLine($"{word}" + node.TerminalValue);
                }
            }
        }

        static void Main(string[] args)
        {
            var program = new Program();
            string input = Console.ReadLine();

            program.Parse();
            //check for FA
            if (program.CheckLetter("S", 0, input, ""))
                Console.WriteLine("Word is valid");
            else Console.Write("Word is not valid");

            //Generate words with lenght Vn + 4 maxim
            program.Generate("S", "");
        }
    }
}
