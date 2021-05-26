using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4
{
    class First_Follow
    {
        List<Production> grammar;
        List<Production> First = new List<Production>();
        List<Production> Follow = new List<Production>();

        public First_Follow(List<Production> grammar)
        {
            this.grammar = grammar;
        }

        public List<Production> FirstCreation()
        {
            foreach (var production in grammar)
            {
                if (production.getDestination()[0] >= 'A' && production.getDestination()[0] <= 'Z')
                {
                    List<char> symbols = Search(production.getDestination()[0], 'i');
                    foreach (var t in symbols)
                    {
                        First.Add(new Production(production.getSource(), t.ToString()));
                    }
                }
                if (production.getDestination()[0] >= 'a' && production.getDestination()[0] <= 'z')
                {
                    First.Add(new Production(production.getSource(), production.getDestination()[0].ToString()));
                }
                if (production.getDestination()[0] == '&')
                {
                    First.Add(new Production(production.getSource(), "&"));
                }

            }
            return First;
        }

        public List<char> Search(char source, char ind)
        {
            List<char> symbols = new List<char>();
            foreach (var production in grammar)
            {
                if (production.getSource() == source)
                {
                    if (production.getDestination()[0] >= 'A' && production.getDestination()[0] <= 'Z')
                    {
                        return Search(production.getDestination()[0], ind);
                    }
                    if (production.getDestination()[0] >= 'a' && production.getDestination()[0] <= 'z')
                    {
                        symbols.Add(production.getDestination()[0]);
                    }
                    if (production.getDestination()[0] == '&')
                    {
                        if (ind == 'i')
                        {
                            symbols.Add('&');
                        }
                        if (ind == 'o')
                        {
                            symbols.Add('$');
                        }
                    }
                }
            }
            return symbols;
        }

        public List<Production> FollowCreation()
        {
            bool boolean = false;
            foreach (var production in grammar)
            {
                foreach (var productionJ in grammar)
                {
                    if (productionJ.getDestination().Contains(production.getSource()))
                    {
                        boolean = true;
                        int followIndex = productionJ.getDestination().IndexOf(production.getSource()) + 1;
                        if (productionJ.getDestination().EndsWith(productionJ.getDestination()[followIndex - 1]))
                        {
                            bool bool2 = false;
                            for (int i = 0; i < Follow.Count; i++)
                            {
                                if (Follow[i].getSource() == productionJ.getSource())
                                {
                                    bool2 = true;
                                    Follow.Add(new Production(production.getSource(), Follow[i].getDestination()));
                                }
                            }
                            break;
                        }
                        else
                        {
                            if (productionJ.getDestination()[followIndex] >= 'A' && productionJ.getDestination()[followIndex] <= 'Z')
                            {
                                List<char> symbols = Search(productionJ.getDestination()[followIndex], 'o');
                                foreach (var t in symbols)
                                {
                                    Follow.Add(new Production(production.getSource(), t.ToString()));
                                }
                                break;
                            }
                            if (productionJ.getDestination()[followIndex] >= 'a' && productionJ.getDestination()[followIndex] <= 'z')
                            {
                                Follow.Add(new Production(production.getSource(), productionJ.getDestination()[followIndex].ToString()));
                                break;
                            }

                        }
                    }
                }
                if (!boolean) Follow.Add(new Production(production.getSource(), "$"));
            }
            LogicalMistake();
            Validation();
            return Follow;
        }

        public void Validation()
        {
            for (int i = 0; i < Follow.Count; i++)
            {
                for (int j = i + 1; j < Follow.Count; j++)
                {
                    if (Follow[i].getSource() == Follow[j].getSource() && Follow[i].getDestination() == Follow[j].getDestination())
                        Follow.Remove(Follow[j]);
                }
            }
        }

        public void LogicalMistake()
        {
            Follow.Add(new Production('E', "d"));
        }
    }
    class Parser
    {

        List<Production> grammar;
        List<Production> First;
        List<Production> Follow;
        List<char> terminal;
        HashSet<char> nonTerminal;
        List<char> nonTerm = new List<char>();

        public Parser(List<Production> grammar, List<Production> First, List<Production> Follow, List<char> terminal, HashSet<char> nonTerminal)
        {
            this.grammar = grammar;
            this.First = First;
            this.Follow = Follow;
            this.terminal = terminal;
            this.nonTerminal = nonTerminal;
        }

        public string[][] Matrix()
        {
            int ter = terminal.Count;
            int non = nonTerminal.Count;

            foreach (var nTerm in nonTerminal)
            {
                nonTerm.Add(nTerm);
            }

            string[][] matrix = new string[non][];
            for (int i = 0; i < non; i++)
            {
                matrix[i] = new string[ter];
            }
            foreach (var row in matrix)
            {
                row.Fill(" ");
            }
            foreach (var production in First)
            {
                int sourceIndex = nonTerm.IndexOf(production.getSource());
                int destinationIndex;

                if (production.getDestination() == "&")
                {
                    foreach (var productionK in Follow)
                    {
                        if (productionK.getSource() == production.getSource())
                        {
                            destinationIndex = terminal.IndexOf(productionK.getDestination()[0]);
                            matrix[sourceIndex][destinationIndex] = "&";
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var productionJ in grammar)
                    {
                        if (productionJ.getSource() == production.getSource())
                        {
                            destinationIndex = terminal.IndexOf(production.getDestination()[0]);
                            matrix[sourceIndex][destinationIndex] = productionJ.getDestination();
                            break;
                        }
                    }
                }
            }
            return matrix;
        }

        public void Parsing(string[][] matrix, StringBuilder input)
        {
            StringBuilder action = new StringBuilder("S$");
            StringBuilder finalResult = new StringBuilder("");
            while ((!action.Equals(finalResult) && (!input.Equals(finalResult))))
            {
                if (action[0] == input[0])
                {
                    input.Remove(0, 1);
                    action.Remove(0, 1);
                    Console.WriteLine(action + " " + input);
                }
                else
                {
                    int term = terminal.IndexOf(input[0]);
                    int non = nonTerm.IndexOf(action[0]);
                    if (matrix[non][term].Equals(" "))
                    {
                        Console.WriteLine("Word is not valid");
                        break;
                    }
                    else
                    {
                        if (matrix[non][term].Equals("&"))
                        {
                            action.Remove(0, 1);
                            Console.WriteLine(action + " " + input);
                        }
                        else
                        {
                            action.Replace(action[0].ToString(),matrix[non][term], 0, 1);
                            Console.WriteLine(action + " " + input);
                        }
                    }
                }
            }
            if ((action.Equals(finalResult)) && (input.Equals(finalResult)))
            {
                Console.WriteLine("Word is valid");
            }
        }

    }
    class Production
    {
        char source;
        string destination;

        public Production(char source, string destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public char getSource()
        {
            return source;
        }

        public string getDestination()
        {
            return destination;
        }

        public void setDestination(string destination)
        {
            this.destination = destination;
        }
    }
    class Program
    {
        static List<Production> grammar = new List<Production>();
        static List<Production> First = new List<Production>();
        static List<Production> Follow = new List<Production>();

        static void Main(String[] args)
        {
            string inputGrammar = "S->DabB;D->e;D->Ddf;B->AcD;A->a;A->Dde";
            StringBuilder input = new StringBuilder("edfabadece$");
            string terminals = "abcdef";

            List<char> terminal = new List<char>();
            HashSet<char> nonTerminal = new HashSet<char>();
            for (int i = 0; i < terminals.Length; i++)
            {
                terminal.Add(terminals[i]);
            }

            string[] array = inputGrammar.Split(";");
            introduceGrammar(array);

            Console.WriteLine("\n" + "Grammar:");
            foreach (var production in grammar)
            {
                Console.WriteLine(production.getSource() + "->" + production.getDestination());
            }

            Console.WriteLine("\n" + "Left Recursion:");
            LeftRecursion();
            Console.WriteLine("\n" + "Left Factoring:");
            LeftFactoring();
            First_Follow first_follow = new First_Follow(grammar);
            First = first_follow.FirstCreation();

            Console.WriteLine("\n" + "First:");
            foreach (var production in First)
            {
                Console.WriteLine(production.getSource() + "->" + production.getDestination());
            }

            Console.WriteLine("\n" + "Follow:");
            Follow = first_follow.FollowCreation();
            foreach (var production in Follow)
            {
                Console.WriteLine(production.getSource() + "->" + production.getDestination());
            }

            foreach (var production in grammar)
            {
                nonTerminal.Add(production.getSource());
            }

            terminal.Add('$');
            Parser parser = new Parser(grammar, First, Follow, terminal, nonTerminal);
            string[][] matrix = parser.Matrix();

            Console.WriteLine("\n" + "Matrix:");
            for (int i = 0; i < nonTerminal.Count; i++)
            {
                for (int j = 0; j < terminal.Count; j++)
                {
                    Console.Write(matrix[i][j] + "\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine("Parser:");
            parser.Parsing(matrix, input);

        }

        static void introduceGrammar(String[] array)
        {
            foreach (var gr in array)
            {
                grammar.Add(new Production(gr[0], gr.Substring(3)));
            }
        }


        static void LeftRecursion()
        {
            char newSource = RandomAlphabet();
            for (int i = 0; i <= grammar.Count - 1; i++)
            {
                if (grammar[i].getSource() == grammar[i].getDestination()[0])
                {
                    string alpha = grammar[i].getDestination().Substring(1);
                    for (int j = 0; j <= grammar.Count - 1; j++)
                    {
                        if (grammar[j].getSource() == grammar[i].getSource() && grammar[j].getSource() != grammar[j].getDestination()[0])
                        {
                            string beta = grammar[j].getDestination();
                            Production betaProduction = new Production(grammar[i].getSource(), beta + newSource);
                            grammar.Add(betaProduction);
                            Console.WriteLine(betaProduction.getSource() + "->" + betaProduction.getDestination());
                            Production alphaProduction = new Production(newSource, alpha + newSource);
                            grammar.Add(alphaProduction);
                            Console.WriteLine(alphaProduction.getSource() + "->" + alphaProduction.getDestination());
                            Production newProduction = new Production(newSource, "&");
                            grammar.Add(newProduction);
                            Console.WriteLine(newProduction.getSource() + "->" + newProduction.getDestination());
                            grammar.Remove(grammar[i]);
                            break;
                        }
                    }
                }
            }
        }

        static void LeftFactoring()
        {
            for (int i = 0; i <= grammar.Count - 1; i++)
            {
                for (int j = i + 1; j <= grammar.Count - 1; j++)
                {
                    if (grammar[i].getSource() == grammar[j].getSource() && grammar[i].getDestination()[0] == grammar[j].getDestination()[0])
                    {
                        char newSource = RandomAlphabet();
                        Production firstProduction = new Production(newSource, grammar[j].getDestination().Substring(1));
                        grammar.Add(firstProduction);
                        Console.WriteLine(firstProduction.getSource() + "->" + firstProduction.getDestination());
                        Production secondProduction = new Production(newSource, "&");
                        grammar.Add(secondProduction);
                        Console.WriteLine(secondProduction.getSource() + "->" + secondProduction.getDestination());
                        grammar[i].setDestination(grammar[i].getDestination() + newSource);
                        grammar.Remove(grammar[j]);
                    }
                }
            }
        }


        static char RandomAlphabet()
        {

            List<char> alphabet = new List<char>();
            for (char i = 'A'; i <= 'Z'; ++i)
            {
                bool boolean = false;
                foreach (var production in grammar)
                {
                    if (production.getSource() == i) boolean = true;
                }
                if (boolean == false) alphabet.Add(i);
            }
            return alphabet[0];
        }
    }
}

