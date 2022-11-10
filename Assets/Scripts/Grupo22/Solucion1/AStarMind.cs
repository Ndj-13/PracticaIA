using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.DataStructures;
using System.Linq;
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
            //var searchResult = Search(boardInfo, currentPos, goals);

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

            // si la Stack no está vacía, hacer siguiente movimient
            //if (currentPlan.Any())
            //{
            //    currentPlan.Pop();
            //}
            //
            //// calcular camino, devuelve resultado de A*
            //var searchResult = Search(boardInfo, currentPos, goals);
            //
            //// recorre searchResult and copia el camino a currentPlan
            //while (searchResult.Parent != null)
            //{
            //    currentPlan.Push(searchResult.ProducedBy);
            //    searchResult = searchResult.Parent;
            //}
            //
            //// returns next move (pop Stack)
            //if (currentPlan.Any())
            //    return currentPlan.Pop();

            return Locomotion.MoveDirection.None; //devuelve nodo donde esta la meta 
                                                  //Cuando tenemos un nodo, para saber q camino ha tomado hay q ver qn es su padre --> recorrer arbol al reves

        }
        
        private Nodo Search (BoardInfo boardInfo, CellInfo start, CellInfo[] goals)
        {
            //Crea lista vacia de nodos
            List<Nodo> openList = new List<Nodo>(); //guarda nodos por recorrer
            ////apunta siempre a primer elemento de la lista
            List<Nodo> openListAux = new List<Nodo>(); //lista para ordenar
            //
            int coste = 0;
            CellInfo[] hijos;            

            //Nodo inicial: posicion del jugador
            Nodo n = new Nodo(boardInfo, start);

            //Añade nodo incial a la lista
            openList.Add(n);

            //Mientras lista no este vacia
            while (openList != null)
            {
                // mira el primer nodo de la lista
                Nodo primerNodoLista = openList[0]; //no queremos crear un nuevo nodo sino acceder al primero de la lista
            
                // si el primer nodo es goal, returns current node
                if(primerNodoLista.posActual == boardInfo.Exit)
                {
                    return primerNodoLista;
                }
                //En este caso como solo hay una salida pdemos usar tanto boardInfo.Exit como gaols[0]
            
                // expande vecinos (calcula coste de cada uno, etc)y los añade en la lista
                hijos = start.WalkableNeighbours(boardInfo); //hijos del nodo
                for(int i = 0; i < hijos.Length; i++)
                {
                    n = new Nodo(boardInfo, hijos[i]); //como nodo ya esta en la lista, actualizamos el valor al de los hijos
                    openList.Add(n); //añadimos cada hijo a la lista
                }

                // ordena lista: comprobar las f y ordenar de menor a mayor
                //Lo metemos en la lista auxiliar de forma ordenada
                //openListAux.Add(openList[0]);
                //for(int i = 1; i < openList.Count; i++) //tenemos el primer nodo + los hijos
                //{
                //    int j = 0; //apunta a la pirmera posicion
                //    while (openListAux[j].compareTo(openList[i]) < 0 && openListAux[j] != null) //lista auxiliar
                //    {
                //        j++;
                //        if(openListAux[j++] != null) //si no es la ultima posicion --> hacer hueco
                //        {
                //            Nodo aux = openListAux[j++];
                //            openListAux[j] = openList[i];
                //
                //        }
                //    }
                //   
                //}
                //ordena lista: comprobar las f y ordenar de menor a mayor
                openList = openList.OrderBy(n => n.f).ToList();
            }
            return null;
        }

    }
}
