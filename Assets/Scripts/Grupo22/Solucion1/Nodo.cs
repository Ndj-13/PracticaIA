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
        private int f;
        private int g = 1;
        CellInfo posActual;
        private CellInfo [] hijos; //entre 0 y 3

        public Nodo(BoardInfo board, CellInfo nodoActual)
        {
            CellInfo meta = board.Exit;
            int columnasMeta = Mathf.Abs(meta.ColumnId - nodoActual.ColumnId);
            int filasMeta = Mathf.Abs(meta.RowId - nodoActual.RowId);

            f = columnasMeta + filasMeta + g; //g = coste de nodo inicial a actual
            //f = distancia desde celda actual + salida + distancia recorrida (g(n) + h*(n)
           posActual = nodoActual;
            //hijos = nodoActual.WalkableNeighbours(board);
        }
    }
}
