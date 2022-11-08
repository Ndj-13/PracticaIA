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
            //apunta siempre a primer elemento de la lista
            List<Nodo> openListAux = new List<Nodo>(); //lista para ordenar

            int coste = 0;
            CellInfo[] hijos;

            //Nodo inicial: posicion del jugador
            Nodo n = new Nodo(board, start);
            //var n = new Nodo()

            //Añade nodo incial a la lista
            openList.Add(n);

            //Mientras lista no este vacia
            while (openList != null)
            {
                // mira el primer nodo de la lista
            
                // si el primer nodo es goal, returns current node
                if(openList[0].Equals(goals))
                {
                    return openList[0];
                }

                // expande vecinos (calcula coste de cada uno, etc)y los añade en la lista
                hijos = start.WalkableNeighbours(board); //hijos del nodo
                for(int i = 0; i < hijos.Length; i++)
                {
                    n = new Nodo(board, hijos[i]); //como nodo ya esta en la lista, actualizamos el valor al de los hijos
                    openList.Add(n); //añadimos cada hijo a la lista
                }

                // ordena lista: comprobar las f y ordenar de menor a mayor
                for(int i = 0; i < openList.Count; i++) //tenemos el primer nodo + los hijos
                {
                    //Nodo a = new Nodo(board, openList.Remove(i));
                   
                }
            }
            return null;
        }

    }
}
