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
            if(currentPlan.Any())
            {
                //Siguiente movimiento
                return currentPlan.Pop(); 
            }

            //calcular camino (algoritmo de busqueda) --> devuelve resultado de A*
            var searchResult = Search(boardInfo, currentPos, goals); //devuelve nodo meta en caso de q haya camino o null en caso de que no

            //Recorre el algoritmo de busqueda y copia el camino en la pila currentPlan
            //Recorre desde el hijo hasta el padre para saber cual es el camino correcto
            while (searchResult.padre != null)
            {
                currentPlan.Push(searchResult.ProducedBy); //metemos en la pila
                searchResult = searchResult.padre; 
            }

            //Devuelve movimiento q tenemos q hacer (pop Pila)
            if (currentPlan.Any())
            {
                return currentPlan.Pop();
            }

            return Locomotion.MoveDirection.None; //devuelve nodo donde esta la meta 
                                                  //Cuando tenemos un nodo, para saber q camino ha tomado hay q ver qn es su padre --> recorrer arbol al reves

        }
        
        private Nodo Search (BoardInfo boardInfo, CellInfo start, CellInfo[] goals)
        {
            //Crea lista vacia de nodos
            List<Nodo> openList = new List<Nodo>(); //guarda nodos por recorrer
            ////apunta siempre a primer elemento de la lista
            CellInfo[] hijos;            

            //Nodo inicial: posicion del jugador
            Nodo n = new Nodo(boardInfo, start, 4, null);
            n.g = 0;

            //Añade nodo incial a la lista
            openList.Add(n);

            //Mientras lista no este vacia
            while (openList.Any())
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
                //[0]: up, [1]: right, [2]: down, [3]: left
                Nodo father = n; //guardamos el valor del padre para pasarselo a los hijos
                for(int i = 0; i < hijos.Length; i++)
                {
                    if (hijos[i].Walkable) //si la posicion es null quiere decir q no se puede ir por ahi
                    {
                        n = new Nodo(boardInfo, hijos[i], i, father); //como nodo ya esta en la lista, actualizamos el valor al de los hijos
                        openList.Add(n); //añadimos cada hijo a la lista
                    }
                }

                //ordena lista: comprobar las f y ordenar de menor a mayor
                openList.RemoveAt(0);
                openList = openList.OrderBy(n => n.f).ToList();
            }
            return null;
        }

    }
}
