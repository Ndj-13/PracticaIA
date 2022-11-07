using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts.Grupo22.Solucion1
{
    public class AStarMind : AbstractPathMind
    {
        public override void Repath()
        {
            currentPlan.Clear(); //ALEX
                                 // limpiar Stack 
        }
        //Algoritmo de busqueda de caminos offline que encuentre la meta (GOAL)

        // declarar Stack de Locomotion.MoveDirection de los movimientos hasta llegar al objetivo
        private Stack<Locomotion.MoveDirection> currentPlan = new Stack<Locomotion.MoveDirection>(); //ALEX 11:30 6/11

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            //Si la pila NO está vacia --> siguiente movimiento

            //calcular camino (algoritmo de busqueda) --> devuelve resultado de A*
            //var algBusq = searchResult(boardInfo, currentPos, goals);

            //Recorre el algoritmo de busqueda y copia el camino en la pila currentPlan
            //while (algBusq.Parent != null)
            //{
            //    currentPlan.Push(algBusq.ProducedBy);
            //    algBusq = algBusq.Parent; 
            //}

            //Devuelve movimiento q tenemos q hacer (pop Pila)
            //if (currentPlan.Any())
            //{
            //    return currentPlan.Pop();
            //}

            return Locomotion.MoveDirection.None; //devuelve nodo donde esta la meta 
                                                  //Cuando tenemos un nodo, para saber q camino ha tomado hay q ver qn es su padre --> recorrer arbol al reves

        }

        private Nodo Search (BoardInfo board, CellInfo start, CellInfo[] goals)
        {
            //Crea lista vacia de nodos
            List<Nodo> openList = new List<Nodo>(); //guarda nodos por recorrer

            int coste = 0;

            //Nodo inicial: posicion del jugador
            Nodo n = new Nodo(board, start, coste);
            //var n = new Nodo()

            //Añade nodo incial a la lista
            openList.Add(n);

            //Mientras lista no este vacia
            //while (openList.Any())
            //{
            //    // mira el primer nodo de la lista
            //
            //    // si el primer nodo es goal, returns current node
            //
            //    // expande vecinos (calcula coste de cada uno, etc)y los añade en la lista
            //
            //    // ordena lista
            //}
            return null;
        }

    }
}
