using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Lab3
{
    class Program
    {
        static readonly string textFile = @"C:\Users\colea\Documents\LFPCLabs\Lab3\Lab3\Varianta9.txt";
        static Dictionary<string, List<string>> P = new Dictionary<string, List<string>>();
        public void Parse()
        {
            using (StreamReader file = new StreamReader(textFile))
            {
                string ln;
                while ((ln = file.ReadLine()) != null)
                {
                    if (P.ContainsKey(ln.Split("-")[0]))
                    {
                        P[ln.Split("-")[0]].Add(ln.Split("-")[1]);
                    }
                    else
                    {
                        P.Add(ln.Split("-")[0], new List<string>() { ln.Split("-")[1] });
                    }
                }
                file.Close();
            }
        }

        public void DeepCopy(ref Dictionary<string, List<string>> newP)
        {
            foreach (var production in P)
            {
                foreach (var productionValue in production.Value)
                {
                    if (newP.ContainsKey(production.Key))
                    {
                        newP[production.Key].Add(productionValue);
                    }
                    else
                    {
                        newP.Add(production.Key, new List<string>() { productionValue });
                    }
                }
            }
        }

        public Dictionary<string, List<string>> RemoveEProductions()
        {
            var newP = new Dictionary<string, List<string>>();
            DeepCopy(ref newP);
            foreach (var production in P)
            {
                foreach (var value in production.Value)
                {
                    if (value == "empty")
                    {
                        foreach (var pr in P)
                        {
                            foreach (var vr in pr.Value)
                            {
                                if (Regex.Matches(vr, production.Key).Count > 0)
                                {
                                    for (int i = 1; i <= Regex.Matches(vr, production.Key).Count; i++)
                                    {
                                        if (!newP[pr.Key].Contains(vr.ReplaceNthOccurrence(production.Key, "", i)))
                                            newP[pr.Key].Add(vr.ReplaceNthOccurrence(production.Key, "", i));
                                    }
                                }
                            }
                        }
                        newP[production.Key].Remove(value);
                    }
                }
            }

            return newP;
        }

        public Dictionary<string, List<string>> RemoveUnitProductions()
        {
            var newP = new Dictionary<string, List<string>>();
            DeepCopy(ref newP);
            foreach (var production in P)
            {
                foreach (var value in production.Value)
                {
                    if (value.Length == 1 && Char.IsUpper(value[0]))
                    {
                        newP[production.Key].Remove(value);
                        List<string> prodValues = P[value];
                        newP[production.Key].AddRange(prodValues);
                        for (int i = 0; i < prodValues.Count; i++)
                        {
                            if (prodValues[i].Length == 1 && Char.IsUpper(prodValues[i][0]))
                            {
                                newP[production.Key].Remove(prodValues[i]);
                                newP[production.Key].AddRange(P[prodValues[i]]);
                            }
                        }
                    }
                }
            }

            return newP;
        }

        public Dictionary<string, List<string>> RemoveInaccessibleVariables()
        {
            var newP = new Dictionary<string, List<string>>();
            DeepCopy(ref newP);
            var accesibilityDictionary = new Dictionary<char, bool>();
            for(char i = 'A'; i <= 'Z'; i++)
            {
                accesibilityDictionary.Add(i, false);
            }
            foreach(var production in P)
            {
                foreach(var value in production.Value)
                {
                    foreach(var character in value)
                    {
                        if (Char.IsUpper(character))
                        {
                            accesibilityDictionary[character] = true;
                        }
                    }
                }
            }

            foreach(var production in P)
            {
                if (!accesibilityDictionary[production.Key[0]])
                {
                    newP.Remove(production.Key);
                }
            }

            return newP;
        }

        public Dictionary<string, List<string>> NormalizeProductions()
        {
            var newP = new Dictionary<string, List<string>>();
            DeepCopy(ref newP);
            foreach (var production in P)
            {
                foreach (var value in production.Value)
                {
                    if (value.Length > 1)
                    {
                        foreach (var character in value)
                        {
                            if (Char.IsLower(character))
                            {
                                if (!newP.ContainsKey(Char.ToUpper(character) + "1"))
                                {
                                    newP.Add(Char.ToUpper(character) + "1", new List<string> { character.ToString() });
                                }
                                var newValue = Regex.Replace(value, character.ToString(), Char.ToUpper(character) + "1");
                                newP[production.Key].Remove(value);
                                newP[production.Key].Add(newValue);
                                break;
                            }
                        }
                    }
                }
            }
            return newP;
        }

        public Dictionary<string, List<string>> ChangeProdctionsWithMoreThanTwoVariables()
        {
            var newP = new Dictionary<string, List<string>>();
            DeepCopy(ref newP);
            foreach (var production in P)
            {
                foreach (var value in production.Value)
                {
                    if (value.Length > 2)
                    {
                        var variable = value;
                        while (variable.Length > 2)
                        {
                            for (char i = 'A'; i <= 'Z'; i++)
                            {
                                if (!newP.ContainsKey(i.ToString()))
                                {
                                    newP.Add(i.ToString(), new List<string> { variable[variable.Length - 2].ToString() + variable[variable.Length - 1] });
                                    variable = variable.Remove(variable.Length - 2);
                                    variable = variable + i;
                                    break;
                                }
                            }
                        }
                        newP[production.Key].Remove(value);
                        newP[production.Key].Add(variable);
                    }
                }
            }
            return newP;
        }

        void PrintChomskyNormalForm()
        {
            foreach(var production in P)
            {
                Console.WriteLine(production.Key.ToString() + "->" + string.Join("|", production.Value));
            }
        }


        static void Main(string[] args)
        {
            var program = new Program();
            program.Parse();
            Console.WriteLine("Initials state");
            program.PrintChomskyNormalForm();
            Console.WriteLine();
            P = program.RemoveInaccessibleVariables();
            Console.WriteLine("Remove inaccesible variables");
            program.PrintChomskyNormalForm();
            Console.WriteLine();
            P = program.RemoveEProductions();
            Console.WriteLine("Remove empty productions");
            program.PrintChomskyNormalForm();
            Console.WriteLine();
            P = program.ChangeProdctionsWithMoreThanTwoVariables();
            Console.WriteLine("Minimize variables");
            program.PrintChomskyNormalForm();
            Console.WriteLine();
            P = program.RemoveUnitProductions();
            Console.WriteLine("Remove unit productions");
            program.PrintChomskyNormalForm();
            Console.WriteLine();
            P = program.NormalizeProductions();
            Console.WriteLine("Chomsky Normal Form");
            program.PrintChomskyNormalForm();
        }
    }
}