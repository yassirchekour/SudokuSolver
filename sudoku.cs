using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA_TP2
{
    class Case
    {
        public int i;
        public int j;
        private int? value;
        private List<Case> relatives;
        public List<int> domain;
        public Case(int i, int j)
        {
            this.i = i;
            this.j = j;
            this.value = 0;
            relatives = new List<Case>();
            domain = new List<int>(Enumerable.Range(1,9).ToArray()); 
        }

        public Case(Case tmp)
        {
            this.i = tmp.i;
            this.j = tmp.j;
            this.value = tmp.value;
            this.relatives=new List<Case>(tmp.relatives);
            this.domain = new List<int>(tmp.domain);
        }

        public int? getValue()
        {
            
            if (domain.Count == 1) setValue(domain[0]);
            return value;
        }
        public void setValue(int value)
        {
            
            this.value = value;
            this.domain.RemoveAll(i => i != value);
        }
        public override String ToString()
        {
            return this.value.ToString();
        }
        public List<Case> getRelatives()
        {
            return relatives;
        }
        public List<Case> getRelativesNonFixed()
        {
            List<Case> tmp = new List<Case>(relatives);
            tmp.RemoveAll(c => c.isFixed());
            return tmp;
        }
        public void addRelative(ref Case relative)
        {
            if (!relatives.Contains(relative) && relative != this)
            {
                this.relatives.Add(relative);
            }
        }
        public bool isFixed()
        {
            if(domain.Count==1 && value!=0)
            {
                return true;
            }
            return false;
        }

        public bool isLost()
        {
            if(domain.Count==0)
            {
                return true;
            }
            return false;
        }

    }
    class Arc
    {
        public Case xi;
        public Case xj;


        public Arc(ref Case xi, ref Case xj)
        {
            this.xi = xi;
            this.xj = xj;
        }
        public Arc(Sudoku sudoku,int i1,int j1, int i2,int j2)
        {
            this.xi = sudoku.mySudoku[i1][j1];
            this.xj = sudoku.mySudoku[i2][j2];
        }
    }
    class Sudoku
    {

        public Case[][] mySudoku { get; set; }
        public int size;
        int subSize = 3;

        // SUBSIZE EST TOUJOURS EGAL A RACINE DE SIZE
        public Sudoku(int size)
        {
            mySudoku = new Case[size][];
            this.size = size;
            if(Math.Sqrt(size)%1!=0)
            {
                //ERROR
                Console.WriteLine("ERROOOOOOR");
                return;
            }
            this.subSize = (int ) Math.Sqrt(size);
            for (int i = 0; i < size; i++)
            {
                mySudoku[i] = new Case[size];
                for (int j = 0; j < size; j++)
                {
                    mySudoku[i][j] = new Case(i, j);
                }
            }
            this.generateRelatives();
        }

        public Sudoku(Sudoku tmp)
        {
            int size = tmp.size;
            mySudoku = new Case[size][];
            this.size = size;
            if(Math.Sqrt(size)%1!=0)
            {
                //ERROR
                Console.WriteLine("ERROOOOOOR");
                return;
            }
            this.subSize = (int ) Math.Sqrt(size);
            for (int i = 0; i < size; i++)
            {
                mySudoku[i] = new Case[size];
                for (int j = 0; j < size; j++)
                {
                    mySudoku[i][j] = new Case(tmp.mySudoku[i][j]);
                }
            }
            this.generateRelatives();
        }
        public Sudoku(Sudoku tmp, bool generate)
        {
            int size = tmp.size;
            mySudoku = new Case[size][];
            this.size = size;
            if (Math.Sqrt(size) % 1 != 0)
            {
                //ERROR
                Console.WriteLine("ERROOOOOOR");
                return;
            }
            this.subSize = (int)Math.Sqrt(size);
            for (int i = 0; i < size; i++)
            {
                mySudoku[i] = new Case[size];
                for (int j = 0; j < size; j++)
                {
                    mySudoku[i][j] = new Case(tmp.mySudoku[i][j]);
                }
            }
            if(generate)
            {
                this.generateRelatives();
            }
        }

        public void generateRelatives()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    generateRelatives(ref mySudoku[i][j]);
                }
            }
        }

        public void generateRelatives(ref Case actCase)
        {
            int actI = actCase.i;
            int actJ = actCase.j;
            for (int i = 0; i < size; i++)
            {
                    actCase.addRelative(ref mySudoku[actI][i]);
                    actCase.addRelative(ref mySudoku[i][actJ]);
                 
            }
            int startingSubI =actI -  actI % subSize;
            int startingSubJ = actJ- actJ % subSize;
            for(int i =startingSubI; i<startingSubI + subSize;i++)
            {
                for(int j = startingSubJ; j <startingSubJ +subSize; j++)
                {
                        actCase.addRelative(ref mySudoku[i][j]);
                    
                }
            }
        }

        public List<Case> getRelatives(int i, int j)
        {
            return mySudoku[i][j].getRelatives();
        }
        public List<Case> getRelativesNonFixed(int i, int j)
        {
            return mySudoku[i][j].getRelativesNonFixed();
        }
    }
}
