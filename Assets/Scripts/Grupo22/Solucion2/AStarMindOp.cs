using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.DataStructures;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Grupo22.Solucion1
{
    public class AStarMindOp : AbstractPathMind
    {
        // declarar Stack de Locomotion.MoveDirection de los movimientos hasta llegar al objetivo
        private Stack<Locomotion.MoveDirection> currentPlan = new Stack<Locomotion.MoveDirection>();
                                                                                                     //Calculamos posiciones enemigos
        //Tenemos prmiero que movernos hacia enemigos y luego hacia meta, creamos lista con todo ello:
        private List<Locomotion.MoveDirection> listaTareas = new List<Locomotion.MoveDirection>();
        
        //private List<CellInfo> enemies = new List<CellInfo>() { };

        public override void Repath()
        {
            currentPlan.Clear(); //ALEX
                                 // limpiar Stack 
        }
        //Algoritmo de busqueda de caminos offline que encuentre la meta (GOAL)

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            //Si la pila NO está vacia --> siguiente movimiento
            while (currentPlan.Any())
            {
                return currentPlan.Pop();
            }

            while (boardInfo.Enemies.Count() > 0)
            {
                var posEnemigo = boardInfo.Enemies[0].GetComponent<EnemyBehaviour>().CurrentPosition();
                //Primero vamos a por los enemigos con busqueda con horizonte
                var searchEnemy = SearchEnemies(boardInfo, currentPos, posEnemigo);

                while (searchEnemy.padre != null)
                {
                    currentPlan.Push(searchEnemy.ProducedBy); //metemos en la pila
                    searchEnemy = searchEnemy.padre;
                }
                if (currentPlan.Any())
                {
                    return currentPlan.Pop();
                }
            }

            var searchResult = SearchGoal(boardInfo, currentPos, goals); //devuelve nodo meta en caso de q haya camino o null en caso de que no

            //Recorre el algoritmo de busqueda y copia el camino en la pila currentPlan
            //Recorre desde el hijo hasta el padre para saber cual es el camino correcto
            while (searchResult.padre != null)
            {
                currentPlan.Push(searchResult.ProducedBy); //metemos en la pila
                searchResult = searchResult.padre;
            }
            if (currentPlan.Any())
            {
                return currentPlan.Pop();
            }
            return Locomotion.MoveDirection.None; //devuelve nodo donde esta la meta
        }

        private Nodo SearchEnemies(BoardInfo boardInfo, CellInfo start, CellInfo enemigo)
        {
            //Se recorre arbol hasta nivel k
            //En nivel k se calculan f* y se busca la menor
            //volviendo a la raiz, se avanza por el nodo hijo q lleve a la f* de menor valor

            //Crea lista vacia de nodos
            List<Nodo> openList = new List<Nodo>(); //guarda nodos por recorrer
            ////apunta siempre a primer elemento de la lista           
            Stack<Nodo> busqHorizonte = new Stack<Nodo>();

            //Nodo inicial: posicion del jugador
            Nodo n = new Nodo(boardInfo, start);
            n.g = 0;

            //Añade nodo incial a la lista
            openList.Add(n);

            //Mientras lista no este vacia
            while (openList.Any())
            {
                //Calcula de que casilla viene el nodo actual (para saber q no puede retroceder)
                if (openList[0].padre != null)
                {
                    openList[0].caminoInverso();
                }
                // mira el primer nodo de la lista --> si es goal, returns current node        
                if (openList[0].posActual == enemigo)
                {
                    return openList[0];
                }
                //En este caso como solo hay una salida pdemos usar tanto boardInfo.Exit como gaols[0]

                for (int k = 0; k < 3; k++) //solo queremos saber 3 primeros niveles
                {
                    //Expandir hijos del nodo actual
                    CellInfo[] hijos = openList[0].posActual.WalkableNeighbours(boardInfo); //hijos del nodo
                                                                                            //[0]: up, [1]: right, [2]: down, [3]: left

                    //Para cada hijo:
                    for(int i = 0; i < hijos.Length; i++)
                    {
                        if (hijos[i] != null)
                        {
                            Nodo h = new Nodo(boardInfo, hijos[i]); //creamos nodo
                            h.padre = openList[0]; //anotamos qn es el padre
                            h.dir = i; //anotamos de q casilla viene
                            if (h.padre != null)
                            {
                                h.g = h.padre.g + hijos[i].WalkCost;
                            }
                            //f solo se calcula para nodos de nivel k = 3
                            if (k == 2) //si estamos en el ultimo nivel de profundidad
                            {
                                h.funHeuristica();
                            }
                            openList.Add(h);
                        }
                    }
                    openList.RemoveAt(0);
                }
                //Una vez tenemos todos los nodos del nivel k, reordenamos lista segun coste
                openList = openList.OrderBy(n => n.f).ToList(); //reordenamos lista y nos quedamos solo con el primero
                //Nos quedamos solo con primer elemento y eliminamos el resto:
                for(int i = 1; i < openList.Count(); i++)
                {
                    openList.RemoveAt(i);
                }

                //Ahora recorremos camino inverso para saber que ruta tomar
                var buscarCamino = openList[0];
                while (buscarCamino.padre != null)
                {
                    busqHorizonte.Push(buscarCamino); //metemos en la pila
                    buscarCamino = buscarCamino.padre;
                }

                //Vaciamos lista y metemos el hijo por el que tiene que ir:
                openList.Clear();
                busqHorizonte.Pop(); //quitamos al padre
                openList.Add(busqHorizonte.Pop());
                busqHorizonte.Clear(); //vaciamos la pila
            }
            return null;
        }

        private Nodo SearchGoal(BoardInfo boardInfo, CellInfo start, CellInfo[] goals)
        {
            //Crea lista vacia de nodos
            List<Nodo> openList = new List<Nodo>(); //guarda nodos por recorrer
            ////apunta siempre a primer elemento de la lista           

            //Nodo inicial: posicion del jugador
            Nodo n = new Nodo(boardInfo, start);
            n.g = 0;

            //Añade nodo incial a la lista
            openList.Add(n);

            //Mientras lista no este vacia
            while (openList.Any())
            {
                //Calcula camino inverso si tiene padre:
                if (openList[0].padre != null)
                {
                    openList[0].caminoInverso();
                }
                // mira el primer nodo de la lista --> si es goal, returns current node        
                if (openList[0].posActual == boardInfo.Exit)
                {
                    return openList[0];
                }
                //En este caso como solo hay una salida pdemos usar tanto boardInfo.Exit como gaols[0]

                // expande vecinos (calcula coste de cada uno, etc)y los añade en la lista
                CellInfo[] hijos = openList[0].posActual.WalkableNeighbours(boardInfo); //hijos del nodo
                                                                                        //[0]: up, [1]: right, [2]: down, [3]: left

                for (int i = 0; i < hijos.Length; i++)
                {
                    if (hijos[i] != null) //si la posicion es null quiere decir q no se puede ir por ahi
                    {
                        Nodo h = new Nodo(boardInfo, hijos[i]);
                        h.padre = openList[0];
                        h.dir = i;
                        if (h.padre != null)
                        {
                            h.g = h.padre.g + hijos[i].WalkCost;
                        }
                        h.funHeuristica();
                        openList.Add(h); //añadimos cada hijo a la lista
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
