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
        int f;
        Nodo sig;
        Nodo ant;

        public Nodo(BoardInfo board, CellInfo posActual, int g)
        {
            CellInfo meta = board.Exit;
            int columnasMeta = Mathf.Abs(meta.ColumnId - posActual.ColumnId);
            int filasMeta = Mathf.Abs(meta.RowId - posActual.RowId);

            int f = columnasMeta + filasMeta + g; //g = coste de nodo inicial a actual
            //f = distancia desde celda actual + salida + distancia recorrida (g(n) + h*(n)
            sig = ant = null;
        }
    }
}
