using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.DataStructures;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Grupo22.Solucion2
{
    public class AscensoAColinas : AbstractPathMind
    {
        // declarar Stack de Locomotion.MoveDirection de los movimientos hasta llegar al objetivo
        private Stack<Locomotion.MoveDirection> currentPlan = new Stack<Locomotion.MoveDirection>();       

        public override void Repath()
        {
            currentPlan.Clear(); 
                                 
        }
        

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            //Si la pila NO está vacia --> siguiente movimiento
            if (currentPlan.Any())
            {
                return currentPlan.Pop();
            }

            List<Locomotion.MoveDirection> siguienteMov = listaMovimientos(boardInfo, currentPos, goals);

            foreach(Locomotion.MoveDirection s in siguienteMov)
            {
                currentPlan.Push(s);
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

            //Buscamos enemigos
            List<NodoOp> enemies = new List<NodoOp>();
            GameObject[] encontrarEnemigos = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject Enemy in encontrarEnemigos)
            {
                NodoOp en = new NodoOp(Enemy.GetComponent<EnemyBehaviour>().CurrentPosition(), null);
                en.f = cercaniaP(currentPos, en.posActual);
                enemies.Add(en);
            }
            enemies = enemies.OrderBy(n => n.f).ToList();

            foreach (NodoOp e in enemies)
            {
                //Primero vamos a por los enemigos con busqueda por ascenso a colinas
                var searchEnemy = SearchEnemies(board, currentPos, e.posActual);

                while (searchEnemy.padre != null)
                {
                    listaTareas.Add(searchEnemy.ProducedBy); //metemos en la lista
                    searchEnemy = searchEnemy.padre;
                }
                return listaTareas;
            }

            //Cuando ya ha encontrado a enemigos:

            var searchResult = SearchGoal(board, currentPos, goals); //devuelve nodo meta en caso de q haya camino o null en caso de que no

            //Recorre el algoritmo de busqueda y copia el camino en la pila currentPlan
            //Recorre desde el hijo hasta el padre para saber cual es el camino correcto
            while (searchResult.padre != null)
            {
                listaTareas.Add(searchResult.ProducedBy); //metemos en la pila
                searchResult = searchResult.padre;
            }
            return listaTareas;
        }

        private int cercaniaP(CellInfo personaje, CellInfo enemigo)
        {
            int columnas = Mathf.Abs(enemigo.ColumnId - personaje.ColumnId);
            int filas = Mathf.Abs(enemigo.RowId - personaje.RowId);
            int res = columnas + filas;
            return res;
        }

        private NodoOp SearchEnemies(BoardInfo boardInfo, CellInfo start, CellInfo enemigo)
        {
            //Crea lista vacia de nodos
            List<NodoOp> openList = new List<NodoOp>(); 
                       
            //Nodo inicial: posicion del jugador
            NodoOp n = new NodoOp(start, enemigo);
            n.g = 0;

            //Añade nodo incial a la lista
            openList.Add(n);

            //Mientras lista no este vacia
            while (openList.Any())
            {
                if (openList[0].padre != null)
                {
                    openList[0].caminoInverso();
                }
                // mira el primer nodo de la lista --> si es goal, returns current node        
                if (openList[0].posActual == enemigo)
                {
                    return openList[0];
                }
                
                //Expandir hijos del nodo actual
                CellInfo[] hijos = openList[0].posActual.WalkableNeighbours(boardInfo); //[0]: up, [1]: right, [2]: down, [3]: left

                //Para cada hijo:
                for (int i = 0; i < hijos.Length; i++)
                {
                    if (hijos[i] != null)
                    {
                        NodoOp h = new NodoOp(hijos[i], enemigo); //creamos nodo
                        h.padre = openList[0]; //anotamos qn es el padre
                        h.dir = i; //anotamos de q casilla viene
                        h.g = 0;
                        h.funHeuristica();
                        //if(comprobarCiclos(openList, h) != true)
                        //{
                            openList.Add(h);
                        //}  
                    }
                }
                openList.RemoveAt(0);
            }
            return null;
        }

        private bool comprobarCiclos(List<NodoOp> lista, NodoOp nodo)
        {
            foreach(NodoOp l in lista)
            {
                if ((l.posActual.ToString() == nodo.posActual.CellId)) return true; //hay ciclo simple
            }
            return false;
        }

        private NodoOp SearchGoal(BoardInfo boardInfo, CellInfo start, CellInfo[] goals)
        {
            //Crea lista vacia de nodos
            List<NodoOp> openList = new List<NodoOp>();           

            //Nodo inicial: posicion del jugador
            NodoOp n = new NodoOp(start, goals[0]);
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
                if (openList[0].posActual == goals[0])
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
                        NodoOp h = new NodoOp(hijos[i], goals[0]);
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
