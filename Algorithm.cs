using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IA_TP2
{
    class Algorithm
    {
        private static Random rnd;
        private static object syncObj = new object();

        private static void InitRandomNumber(int seed)
        {
            rnd = new Random(seed);
        }
        private static int GenerateRandomNumber(int max)
        {
            lock (syncObj)
            {
                if (rnd == null)
                {
                    rnd = new Random();
                }
                return rnd.Next(0, max);
            }

        }
        public static Sudoku ReadCSV()
        {
            Sudoku sudoku = new Sudoku(9);
            int i = 0;
            int rnd = GenerateRandomNumber(5);
            MainWindow.num = rnd + 1;
            StreamReader reader;

            
            switch (rnd)
            {
                case 0:
                    reader = new StreamReader(@"sudoku1.csv");
                    MainWindow.diff = "Easy";
                    break;
                case 1:
                    reader = new StreamReader(@"sudoku2.csv");
                    MainWindow.diff = "Medium";
                    break;
                case 2:
                    reader = new StreamReader(@"sudoku3.csv");
                    MainWindow.diff = "Hard";
                    break;
                case 3:
                    reader = new StreamReader(@"sudoku4.csv");
                    MainWindow.diff = "Hell";
                    break;
                case 4:
                    reader = new StreamReader(@"sudoku5.csv");
                    MainWindow.diff = "Hard";
                    break;
                default:
                    reader = new StreamReader(@"sudoku1.csv");
                    break;
            }
            

            //reader = new StreamReader(@"test.csv");
            //Remplacer par adresse du fichier
            using (reader)
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    for (int j = 0; j < 9; j++)
                    {
                        if(Int16.Parse(values[j])!=0)
                        sudoku.mySudoku[i][j].setValue(Int16.Parse(values[j]));
                    }
                    i = i + 1;
                }
            }

            return sudoku;

        }

        public static Sudoku ReadCSVFromFile(string str)
        {
            Sudoku sudoku = new Sudoku(9);
            int i = 0;

            StreamReader reader = new StreamReader(str);

            using (reader)
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    for (int j = 0; j < 9; j++)
                    {
                        if (Int16.Parse(values[j]) != 0)
                            sudoku.mySudoku[i][j].setValue(Int16.Parse(values[j]));
                    }
                    i = i + 1;
                }
            }


            return sudoku;
        }

        public static void AC3(Sudoku sudoku)
        {

            Queue<Arc> queue = new Queue<Arc>();

            //ADD ALL ARC TO QUEUE
            for (int i = 0; i < sudoku.size; i++)
            {
                for (int j = 0; j < sudoku.size; j++)
                {
                    
                    for (int index = 0; index < sudoku.getRelatives(i, j).Count; index++)
                    {
                        Case actCase = sudoku.mySudoku[i][j].getRelatives()[index];
                        queue.Enqueue(new Arc(sudoku, i, j, actCase.i, actCase.j));
                    }
                }
            }
            while (queue.Count > 0)
            {
                Arc actArc = queue.Dequeue();
                if (removeInconsistentValues(actArc))
                {
                    for (int index = 0; index < actArc.xi.getRelatives().Count; index++)
                    {
                        Case actCase = actArc.xi.getRelatives()[index];
                        queue.Enqueue(new Arc(sudoku, actCase.i, actCase.j,actArc.xi.i,actArc.xi.j));
                    }
                }
            }


        }


        /**
         * Si une valeur de xi n'est possible avec aucune des valeurs du domaines de xj alors on la supprime du domaine de xi
         */
        public static bool removeInconsistentValues(Arc arc)
        {
            bool removed = false;
            int length = arc.xi.domain.Count;
            int valueI;
            int k = 0;
            for (int i =0; i<length; i++)
            {
                valueI = arc.xi.domain.ElementAt(k);
                bool allow = false;
                foreach (int valueJ in arc.xj.domain)
                {
                    if (valueJ != valueI)
                    {
                        allow = true;
                    }
                }
                if (!allow)
                {
                    arc.xi.domain.Remove(valueI);
                    removed = true;
                }
                else { k += 1; }
            }
            return removed;
        }

        /**
         * On selectionne la variable qui a le moins de valeurs légales et on renvoie sa position i,j
         */
        public static List<(int, int)> selectMRV(Sudoku sudoku)
        {
            int valmin = 10;
            List<(int, int)> ret = new List<(int, int)>();

            for (int indexI = 0; indexI < sudoku.size; indexI++)
            {
                for (int indexJ = 0; indexJ < sudoku.size; indexJ++)
                {

                    if ((valmin > sudoku.mySudoku[indexI][indexJ].domain.Count) && !(sudoku.mySudoku[indexI][indexJ].isFixed()))
                    {
                        valmin = sudoku.mySudoku[indexI][indexJ].domain.Count;
                    }
                }
            }
            for (int indexI = 0; indexI < sudoku.size; indexI++)
            {
                for (int indexJ = 0; indexJ < sudoku.size; indexJ++)
                {
                    if ((valmin == sudoku.mySudoku[indexI][indexJ].domain.Count) && !(sudoku.mySudoku[indexI][indexJ].isFixed()))
                    {
                        ret.Add((indexI, indexJ));
                    }
                }
            }

            return ret;
        }

        /**
         * On selectionne la variable qui contraint le plus de variables et on renvoie sa position i,j
         * On appelle selectDH après avoir appelé selectMRV
         */
        public static (int, int) selectDH(Sudoku sudoku, List<(int, int)> resultMRV)
        {
            int i = resultMRV.ElementAt(0).Item1;
            int j = resultMRV.ElementAt(0).Item2;
            (int, int) current;
            int length = resultMRV.Count;
            for (int k = 0; k < length; k++)
            {
                current = resultMRV.ElementAt(k);
                if (sudoku.mySudoku[i][j].getRelativesNonFixed().Count < sudoku.mySudoku[current.Item1][current.Item2].getRelativesNonFixed().Count)
                {
                    i = current.Item1;
                    j = current.Item2;
                }
            }
            return (i, j);
        }

        /*
         On selectionne la valeur pour la variable déjà choisie qui réduit le moins les valeurs possibles des prochaines variables (sudoku.getRelatives(var))
        */
        public static int selectLCV(Sudoku sudoku,(int,int) var)
        {
            int i = var.Item1;
            int j = var.Item2;

            int minVal = 0;
            int somme = 0;
            int sommeMin = 9999;

            bool impossible = false;

            Sudoku tmp = new Sudoku(sudoku);

            // On parcourt les différentes valeurs possibles de la variable de base

            foreach (int value in sudoku.mySudoku[i][j].domain)
            {
                // On applique la valeur value
                
                tmp = new Sudoku(sudoku);
                tmp.mySudoku[i][j].setValue(value);
                AC3(tmp);

                // On parcourt les variables relatives

                foreach (Case relative in tmp.getRelativesNonFixed(i,j))
                {
                    // On somme la taille de tous les domaines (si différente de 0)

                    if (tmp.mySudoku[relative.i][relative.j].domain.Count == 0)
                    {
                        impossible = true;
                    }
                    else
                    {
                        somme += tmp.mySudoku[relative.i][relative.j].domain.Count;
                    }
                }

                // Si la somme des tailles de domaine est minimale, on garde cette valeur

                if(somme < sommeMin && impossible == false)
                {
                    sommeMin = somme;
                    minVal = value;
                }
                somme = 0;
                impossible = false;
            }

            if(minVal == 0)
            {
                Console.WriteLine("Aucune valeur possible pour cette case");
            }
            return minVal;
        }

        //On test si une solution est encore possible
        public static bool isLost(Sudoku sudoku)
        {
             for (int i = 0; i < sudoku.size; i++)
            {
                for (int j = 0; j < sudoku.size; j++)
                {
                    if (sudoku.mySudoku[i][j].isLost()) {return true;}
                }
            }
             return false;
        }

        //On test si une solution est trouvée
        public static bool isFinished(Sudoku sudoku)
        {
            for (int i = 0; i < sudoku.size; i++)
            {
                for (int j = 0; j < sudoku.size; j++)
                {
                    if (!sudoku.mySudoku[i][j].isFixed()) { return false; }
                }
            }
            return true;
        }


        public static Sudoku backtracking(Sudoku sudoku)
        {
            Sudoku result = new Sudoku(sudoku);
            Sudoku tmp = new Sudoku(sudoku);
            bool flag;
            AC3(sudoku);

            // 1 - Selectionner var non assigné : utiliser MRV puis DH en cas d'égalité
            (int, int) var;
            var = selectDH(sudoku, selectMRV(sudoku));

            int l = sudoku.mySudoku[var.Item1][var.Item2].domain.Count;

            for (int i=0; i<l; i++)
            {
                // 2 - Selectionner val pour var choisie : utiliser LCV
                int val = selectLCV(sudoku, var);
                AC3(sudoku);
                sudoku.mySudoku[var.Item1][var.Item2].setValue(val);
                
                Console.WriteLine("Affectation de la case i : "+ var.Item1 + " j: "+ var.Item2 + " avec la valeur : "+val);

                // 3 - AC3 => refresh du domaine de chaque case
                AC3(sudoku);

                //detection de l'échec
                if (isLost(sudoku))
                {
                    // le resultat trouvé n'était pas viable => on revient en arrière
                    sudoku = new Sudoku(tmp);
                    sudoku.mySudoku[var.Item1][var.Item2].domain.Remove(val);
                }
                else
                {
                    // 4 - Recursivité
                    try
                    {
                        flag = true;
                        if (!isFinished(sudoku))
                        {
                            result = backtracking(sudoku);
                        }
                        else
                        {
                            result = sudoku;
                        }
                    }
                    catch (Failure ex)
                    {
                        // le resultat trouvé n'était pas viable => on revient en arrière  
                        sudoku = new Sudoku(tmp);
                        sudoku.mySudoku[var.Item1][var.Item2].domain.Remove(val);
                        flag = false;

                    }
                    if (flag)
                    {
                        //RETURN SOLUTION ICI
                        return result;
                    }
                }
            }
            //Failure
            throw new Failure("Pas de solution il faut remonter");
            return result;
        }

        public class Failure : Exception
        {
            public Failure(string message)
                :base(message)
            {

            }
        }
    }


}