using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Runtime.Remoting.Messaging;
using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts.Grupo22.Solucion1
{

    public class Nodo
    {
        //Variables heuristica:
        public double f;
        public double g;
        public CellInfo meta;
        public CellInfo posActual;

        public Nodo padre;
        public int dir;
        
        public Locomotion.MoveDirection ProducedBy;

        public Nodo(BoardInfo board, CellInfo nodoActual)
        {
            posActual = nodoActual;
            meta = board.Exit;
        }

        public double funHeuristica()
        {
            //int columnasMeta = Mathf.Abs(meta.ColumnId - posActual.ColumnId);
            //int filasMeta = Mathf.Abs(meta.RowId - posActual.RowId);
            Vector2 distManhattam = meta.GetPosition - posActual.GetPosition;
            double h = Mathf.Abs(distManhattam.x) + Mathf.Abs(distManhattam.y);

            f = h + g;

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

            //Funcion heuristica de A*: f*(n) = g(n) + h*(n)

            //h*(n): coste estimado de nodo actual a mejor meta
            //CellInfo meta = board.Exit;
            //int columnasMeta = Mathf.Abs(meta.ColumnId - nodoActual.ColumnId);
            //int filasMeta = Mathf.Abs(meta.RowId - nodoActual.RowId);
            //
            ////g = coste de nodo inicial a actual
            //g = father.g + nodoActual.WalkCost;
            //
            //f = columnasMeta + filasMeta + g; 
            ////f = distancia desde celda actual + salida + distancia recorrida (g(n) + h*(n)
            //
            //posActual = nodoActual;
            //hijos = nodoActual.WalkableNeighbours(board);
            //padre = father;
            //dir = direccion;
            //switch(direccion)
            //{
            //    case 0:
            //        ProducedBy = Locomotion.MoveDirection.Up;
            //        break;
            //    case 1:
            //        ProducedBy = Locomotion.MoveDirection.Right;
            //        break;
            //    case 2:
            //        ProducedBy = Locomotion.MoveDirection.Down;
            //        break;
            //    case 3:
            //        ProducedBy = Locomotion.MoveDirection.Left;
            //        break;
            //    case 4:
            //        ProducedBy = 0; //para la raiz
            //        break;
            //}
        
    }
}
