using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.DataStructures;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Grupo22.Solucion2
{
    public class BusquedaHorizonte : AbstractPathMind
    {
        private Stack<Locomotion.MoveDirection> currentPlan = new Stack<Locomotion.MoveDirection>();
        
        public override void Repath()
        {
            currentPlan.Clear(); 
        }

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            //Si la pila NO esta vacia --> siguiente movimiento
            if (currentPlan.Any())
            {
                return currentPlan.Pop();
            }

            List<Locomotion.MoveDirection> siguienteMov = listaMovimientos(boardInfo, currentPos, goals);

            while (siguienteMov.Any())
            {
                currentPlan.Push(siguienteMov[0]);
                siguienteMov.RemoveAt(0);
            }

            if (currentPlan.Any())
            {
                return currentPlan.Pop();
            }

            return Locomotion.MoveDirection.None; //devuelve nodo donde esta la meta
        }

        private List<Locomotion.MoveDirection> listaMovimientos(BoardInfo board, CellInfo currentPos, CellInfo[] goals)
        {
            List<Locomotion.MoveDirection> listaTareas = new List<Locomotion.MoveDirection>();
            //Metemos movimientos

            //Buscamos enemigos
            List<NodoOp> enemies = new List<NodoOp>();
            GameObject[] encontrarEnemigos = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject Enemy in encontrarEnemigos)
            {
                NodoOp en = new NodoOp(Enemy.GetComponent<EnemyBehaviour>().CurrentPosition(), null);
                en.f = cercaniaP(currentPos, en.posActual);
                enemies.Add(en);
            }
            enemies = enemies.OrderBy(n => n.f).ToList();

            if (enemies.Any())
            { 
                var searchEnemy = SearchEnemies(board, currentPos, enemies[0].posActual);

                while (searchEnemy.padre != null)
                {
                    listaTareas.Add(searchEnemy.ProducedBy); //metemos en la lista
                    searchEnemy = searchEnemy.padre;
                }
                return listaTareas;
            }

            //Cuando ya ha encontrado a enemigos:

            var searchResult = SearchGoal(board, currentPos, goals); 

            while (searchResult.padre != null)
            {
                listaTareas.Add(searchResult.ProducedBy); 
                searchResult = searchResult.padre;
            }
            return listaTareas;
        }

        private NodoOp SearchEnemies(BoardInfo boardInfo, CellInfo start, CellInfo enemigo)
        {
            //Se recorre arbol hasta nivel k --> En nivel k se calculan f* y se busca la menor
            //volviendo a la raiz, se avanza por el nodo hijo q lleve a la f* de menor valor

            //Crea lista vacia de nodos
            List<NodoOp> openList = new List<NodoOp>(); 
                       
            Stack<NodoOp> busqHorizonte = new Stack<NodoOp>();

            //Nodo inicial: posicion del jugador
            NodoOp n = new NodoOp(start, enemigo);
            n.g = 0;

            //Anade nodo incial a la lista
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

                for (int k = 0; k < 3; k++) //solo queremos saber 3 primeros niveles
                {
                    //Expandir hijos del nodo actual
                    CellInfo[] hijos = openList[0].posActual.WalkableNeighbours(boardInfo); //[0]: up, [1]: right, [2]: down, [3]: left

                    //Para cada hijo:
                    for (int i = 0; i < hijos.Length; i++)
                    {
                        if (hijos[i] != null)
                        {
                            NodoOp h = new NodoOp(hijos[i], enemigo); 
                            h.padre = openList[0]; 
                            h.dir = i; 
                            if (h.padre != null)
                            {
                                h.g = h.padre.g + hijos[i].WalkCost;
                            }
                            //f solo se calcula para nodos de ultimo nivel de profundidad
                            if (k == 2) 
                            {
                                h.funHeuristica();
                            }
                            openList.Add(h);
                        }
                        hijos[i] = null;
                    }
                    openList.RemoveAt(0);
                }
                //Una vez tenemos todos los nodos del nivel k, reordenamos lista segun coste
                openList = openList.OrderBy(n => n.f).ToList(); 

                //Nos quedamos solo con primer elemento y eliminamos el resto:
                for (int i = openList.Count() - 1; i > 0; i--)
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
                busqHorizonte.Pop(); //quitamos al padre
                openList.Add(busqHorizonte.Pop());
                openList.RemoveAt(0);
                busqHorizonte.Clear(); //vaciamos la pila
            }
            return null;
        }

        private NodoOp SearchGoal(BoardInfo boardInfo, CellInfo start, CellInfo[] goals)
        {
            //Crea lista vacia de nodos
            List<NodoOp> openList = new List<NodoOp>();           

            //Nodo inicial: posicion del jugador
            NodoOp n = new NodoOp(start, goals[0]);
            n.g = 0;

            //Anade nodo incial a la lista
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
                if (openList[0].posActual == goals[0])
                {
                    return openList[0];
                } 

                // expande vecinos (calcula coste de cada uno, etc)y los anade en la lista
                CellInfo[] hijos = openList[0].posActual.WalkableNeighbours(boardInfo); //hijos del nodo
                                                                                        //[0]: up, [1]: right, [2]: down, [3]: left

                for (int i = 0; i < hijos.Length; i++)
                {
                    if (hijos[i] != null) 
                    {
                        NodoOp h = new NodoOp(hijos[i], goals[0]);
                        h.padre = openList[0];
                        h.dir = i;
                        if (h.padre != null)
                        {
                            h.g = h.padre.g + hijos[i].WalkCost;
                        }
                        h.funHeuristica();
                        openList.Add(h); 
                    }
                }

                //ordena lista: comprobar las f y ordenar de menor a mayor
                openList.RemoveAt(0);
                openList = openList.OrderBy(n => n.f).ToList();
            }
            return null;
        }

        private int cercaniaP(CellInfo personaje, CellInfo enemigo)
        {
            int columnas = Mathf.Abs(enemigo.ColumnId - personaje.ColumnId);
            int filas = Mathf.Abs(enemigo.RowId - personaje.RowId);
            int res = columnas + filas;
            return res;
        }

    }
}
