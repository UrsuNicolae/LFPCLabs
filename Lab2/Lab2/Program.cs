using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lab2
{
    class Node
    {
        public string NextState { get; set; }

        public string Value { get; set; }

    }
    class Program
    {
        static readonly string textFile = @"C:\Users\nicolae.ursu\Documents\LFPCLabs\Lab2\Lab2\Varianta9.txt";
        static string[] Q;
        static string[] E;
        static string[] start;
        static string[] final;
        static Dictionary<string, List<Node>> NFA = new Dictionary<string, List<Node>>();
        static Queue<string> Transitions = new Queue<string>();

        static Dictionary<string, List<Node>> DFA = new Dictionary<string, List<Node>>();

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
                            Q = ln.Split(' ');
                            break;
                        case 2:
                            E = ln.Split(' ');
                            break;
                        case 3:
                            final = ln.Split(" ");
                            break;
                        case 4:
                            start = ln.Split(" ");
                            foreach (var s in start)
                            {
                                Transitions.Enqueue(s);
                            }
                            break;
                        default:

                            var transformations = ln.Split(' ');
                            if (!NFA.ContainsKey(transformations[0].Split("-")[0]))
                                NFA.Add(transformations[0].Split("-")[0], new List<Node>());
                            NFA[transformations[0].Split("-")[0]].Add(new Node
                            {
                                NextState = transformations[2],
                                Value = transformations[0].Split('-')[1]
                            });


                            break;

                    }
                }
                file.Close();
            }
        }

        public void ConverNFAToDFA()
        {
            while (Transitions.Any())
            {
                if (!DFA.ContainsKey(Transitions.Peek()))
                {
                    DFA.Add(Transitions.Peek(), new List<Node>());
                }
                for (int i = 0; i < E.Length; i++)
                {
                    var currentTransition = new List<Node>();
                    var trans = Transitions.Peek().Split('-');
                    if (trans.Count() > 1)
                    {
                        foreach (var tr in trans)
                        {
                            if (tr != "DEAD")
                            {
                                foreach(var transition in NFA[tr].Where(x => x.Value == E[i]).ToList())
                                    currentTransition.Add(transition);
                            }
                        }
                    }
                    if (NFA.ContainsKey(Transitions.Peek()))
                    {

                        currentTransition = NFA[Transitions.Peek()].Where(x => x.Value == E[i]).ToList();
                    }
                    //-----new q2q3 reunion need to be made
                    if (currentTransition.Count() > 0)
                    {
                        var newState = String.Concat(currentTransition.Select(x => x.NextState + "-"));
                        newState = newState.TrimEnd('-');
                        DFA[Transitions.Peek()].Add(new Node
                        {
                            NextState = newState,
                            Value = E[i]
                        });
                        if (!DFA.ContainsKey(newState))
                        {
                            Transitions.Enqueue(newState);
                            DFA.Add(newState, new List<Node>());
                        }
                    }
                    else
                    {
                        if (!DFA.ContainsKey("DEAD"))
                        {
                            Transitions.Enqueue("DEAD");
                            DFA.Add("DEAD", new List<Node>());
                        }
                        DFA[Transitions.Peek()].Add(new Node
                        {
                            NextState = "DEAD",
                            Value = E[i]
                        });
                    }
                }
                Transitions.Dequeue();
            }
        }

        /// <summary>
        /// DEAD means this is a dead state
        /// -> point to the start positions
        /// * point to the final positions
        /// </summary>
        public void PrindDFA()
        {
            for (int i = 0; i < E.Length; i++)
            {
                Console.Write("\t\t");
                Console.Write(E[i]);
            }
            Console.WriteLine();
            foreach(var transition in DFA)
            {
                if (transition.Key.Contains("-"))
                {
                    var transitions = transition.Key.Split("-");
                    var printed = false;
                    foreach(var tr in transitions)
                    {
                        if (start.Contains(tr) && final.Contains(tr))
                        {
                            Console.Write("->*");
                            foreach (var startEndTransition in transitions)
                                Console.Write(startEndTransition);
                            printed = true;
                            break;
                        }
                        else if (start.Contains(tr))
                        {
                            Console.Write("->");
                            foreach (var startEndTransition in transitions)
                                Console.Write(startEndTransition);
                            printed = true;
                            break;
                        }
                        else if (final.Contains(tr))
                        {
                            Console.Write("*");
                            foreach (var startEndTransition in transitions)
                                Console.Write(startEndTransition);
                            printed = true;
                            break;
                        }
                    }
                    if(!printed)
                    {
                        foreach (var startEndTransition in transitions)
                            Console.Write(startEndTransition);
                    }
                }
                else
                {
                    if (start.Contains(transition.Key) && final.Contains(transition.Key))
                    {
                        Console.Write($"->*{transition.Key}");

                    }
                    if (start.Contains(transition.Key))
                    {
                        Console.Write($"->{transition.Key}");

                    }
                    else if (final.Contains(transition.Key))
                    {
                        Console.Write($"*{transition.Key}");
                    }
                    else
                    {
                        Console.Write($"{transition.Key}");
                    }
                }
                
                for (int j = 0; j < E.Length; j++)
                {
                    foreach (var tr in transition.Value)
                    {
                        if (tr.Value == E[j])
                        {
                            Console.Write("\t\t");
                            Console.Write(tr.NextState);
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        
        static void Main(string[] args)
        {
            var program = new Program();
            program.Parse();
            program.ConverNFAToDFA();
            program.PrindDFA();
            
        }
    }
}
