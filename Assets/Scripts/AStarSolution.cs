using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts.AStarSolution
{
    public class AStartMind : AbstractPathMind
    {
        // declarar Stack de Locomotion.MoveDirection de los movimientos hasta llegar al objetivo
        private Stack<Locomotion.MoveDirection> currentPlan = new Stack<Locomotion.MoveDirection>(); //ALEX 11:30 6/11

        public override void Repath()
        {
            currentPlan.Clear(); //ALEX
            // limpiar Stack 
        }

        public override Locomotion.MoveDirection GetNextMove(BoardInfo board, CellInfo currentPos, CellInfo[] goals)
        {
            // si la Stack no está vacía, hacer siguiente movimiento
            if (currentPlan != null) //ALEX
            {
                //NO
                return currentPlan.Pop();
                // devuelve siguiente movimient 
            }

            // calcular camino, devuelve resultado de A*
            var searchResult = Search(board, currentPos, goals);

            // recorre searchResult and copia el camino a currentPlan
            while (searchResult.Parent != null)
            {
                currentPlan.Push(searchResult.ProducedBy);
                searchResult = searchResult.Parent;
            }

            // returns next move (pop Stack)
            if (currentPlan.Any())
                return currentPlan.Pop();

            return Locomotion.MoveDirection.None;

        }

        private Node Search(BoardInfo board, CellInfo start, CellInfo[] goals)
        {
            // crea una lista vacía de nodos
            var open = new List<Node> listaVNodos = new List<Node>(); //MIRAR QUE ES NODE ALEX

            // node inicial
            var n = new ...

            // añade nodo inicial a la lista
            open.Add(n);

            // mientras la lista no esté vacia
            while (open.Any())
            {
                // mira el primer nodo de la lista

                // si el primer nodo es goal, returns current node

                // expande vecinos (calcula coste de cada uno, etc)y los añade en la lista

                // ordena lista
            }
            return null;
        }
    }
}