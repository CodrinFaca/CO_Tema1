using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace simplex_gui
{
    public class Pivot
    {
        public int Column { get; set; }
        public string variableName { get; set; }
        public int Row { get; set; }
    }
    public class Solver
    {
        double[,] matrix;
        int varNo;
        int constrNo;

        List<string> constrVarNames = new List<string>(); //s1..si
        List<string> allVarNames = new List<string>(); //x1..xi,s1..si

        public Solver(double[,] matrix, int varNo, int constrNo)
        {
            this.matrix = matrix;
            this.varNo = varNo;
            this.constrNo = constrNo;

            this.ConstrVarNames.Add("z");
            for (int i = 0; i < varNo + constrNo; i++)
            {
                if (i >= varNo)
                {
                    string str = "s" + (i - varNo + 1);
                    allVarNames.Add(str);
                    ConstrVarNames.Add(str);
                }
                else
                {
                    string str = "x" + (i + 1);
                    allVarNames.Add(str);
                }
            }
            allVarNames.Add("RHS");
        }

      
        public double getMatrixValue(int row, int column) => matrix[row, column];
        public List<string> getAllVarNames => allVarNames;
        public List<string> getConstructedVarNames => ConstrVarNames;

        public List<string> ConstrVarNames { get => constrVarNames; set => constrVarNames = value; }

        public bool IsOptimal()
        {
            // if tm+1;k > 0, 8k, the simplex table is optimal: the current basic feasible solution is optimal.
            //every column in first row is >= 0.
            for (int k=0; k< allVarNames.Count;k++)
            {
                //solution is not optimal
                if (matrix[0, k] < 0)
                    return false;
            }
            return true;
        }

        public double getResult()
        {
            return -matrix[0, matrix.GetLength(1) - 1];
        }
        public Pivot getPivot()
        {
            //col name starts with "z"
            for(int p = 0; p < allVarNames.Count; p++)
            {
                if (matrix[0, p] < 0)
                    return new Pivot() { Column = p, variableName = allVarNames[p] };
            }
            return null;
        }

        public bool IsUnbound(Pivot p)
        {
            //unbound if all elements on pivot column are <=0 
            for (int i = 1; i < matrix.GetLength(0); i++)
                if (matrix[i, p.Column] > 0)
                    return false;
            return true;
        }
        public int getPivotRow(Pivot p)
        {
            var minValue = double.MaxValue;
            var row_index = -1;
            var RhsCol = matrix.GetLength(1) - 1;
            for(int row = 1; row < matrix.GetLength(0); row++)
            {
                if (matrix[row, p.Column] <= 0)
                    continue;
                var val = matrix[row, RhsCol] / matrix[row, p.Column];
                if(val < minValue)
                {
                    minValue = val;
                    row_index = row;
                }
            }
            return row_index;
        }

        public void DoPivoting(Pivot p)
        {
            //rename row to pivot value
            getConstructedVarNames[p.Row] = p.variableName;
            //k = p.row
            //l = p.column
            var pivotElem = matrix[p.Row, p.Column];
            for(int i = 0; i< matrix.GetLength(0); i++)
            {
                if (i == p.Row)
                    continue;
                for(int j = 0;j < matrix.GetLength(1);j++)
                {
                        if (j == p.Column)
                        continue;
                    ////pivoting rule
                    //var newVal = ((matrix[i, j] * pivotElem) -
                    //    (matrix[i, p.Column] * matrix[p.Row, j])) / pivotElem;
                    var newVal = (matrix[p.Row,j] * (matrix[i, p.Column] / -pivotElem)) + matrix[i, j];
                    matrix[i, j] = newVal;
                }
            }

            for(int i = 0; i< matrix.GetLength(0); i++)
            {
                if (i == p.Row)
                    continue;
                matrix[i, p.Column] = 0;
            }
            for(int j = 0; j < matrix.GetLength(1); j++)
            {
                if (j == p.Column)
                    continue;
                matrix[p.Row, j] = matrix[p.Row, j] / pivotElem;
            }
            matrix[p.Row, p.Column] = 1;
        }
    }
}
