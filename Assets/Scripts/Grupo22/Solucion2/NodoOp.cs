using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Runtime.Remoting.Messaging;
using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts.Grupo22.Solucion1
{

    public class NodoOp
    {
        //Variables heuristica:
        public double f;
        public double g;
        public CellInfo meta;
        public CellInfo posActual;

        public Nodo padre;
        public int dir;

        public Locomotion.MoveDirection ProducedBy;

        public NodoOp(BoardInfo board, CellInfo nodoActual)
        {
            posActual = nodoActual;
            meta = board.Exit;
        }

        public double funHeuristica()
        {
            int columnasMeta = Mathf.Abs(meta.ColumnId - posActual.ColumnId);
            int filasMeta = Mathf.Abs(meta.RowId - posActual.RowId);

            f = columnasMeta + filasMeta + g; //h = columnas + filas

            return f;
        }

        public void caminoInverso()
        {
            switch (dir)
            {
                case 0:
                    ProducedBy = Locomotion.MoveDirection.Up;
                    break;
                case 1:
                    ProducedBy = Locomotion.MoveDirection.Right;
                    break;
                case 2:
                    ProducedBy = Locomotion.MoveDirection.Down;
                    break;
                case 3:
                    ProducedBy = Locomotion.MoveDirection.Left;
                    break;
            }
        }

    }
}
