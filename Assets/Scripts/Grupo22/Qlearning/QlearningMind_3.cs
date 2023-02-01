using Assets.Scripts.DataStructures;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

namespace Assets.Scripts.PracticaPart2
{

    public class QlearningMind_3 : AbstractPathMind{
        //public FileStream ficherotablaQ;

        private float[,] tableQ; 

        public override void Repath()
        {
            
        }
        
        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            //Crear fichero
            //writeTableQ();
            //Debug.Log(currentPos.GetPosition);

            if (tableQ == null)
            {
                tableQ = new float[8 * 14, 4];
                //Debug.Log(tableQ[0,0]);
                //ficherotablaQ = createTableQ(boardInfo);
                aprendizajeQ(goals, boardInfo);
                

            } else
            {
                //arrayTablaQ("./Assets/Scripts/Grupo22/Qlearning/tableQ.txt");
                //ficherotablaQ = new FileStream("./Assets/Scripts/PracticaPart2/tableQ.txt", FileMode.Open, FileAccess.Read);
                return explotar(boardInfo, currentPos, goals);
            }


            return Locomotion.MoveDirection.None;
        }

        public void aprendizajeQ(CellInfo[] goals, BoardInfo boardInfo)
        { //seguramente necesitemos varios atributos como boiardinfo
            //------------------------------------------------- APRENDIZAJE con EXPLORACION 100%
            //float[,] tableQ = new float[8*14, 4]; //ponerlo mejor donde se escribe el txt
            string path = "./Assets/Scripts/Grupo22/Qlearning/tableQ.txt";
            //createTableQ(boardInfo, path);
            //tableQ = arrayTablaQ(path);

            int N_EPI_MAX = 100;
            int N_ITER_MAX = 100;
            for (int k = 0; k < N_EPI_MAX; k++)
            {
                int iter = 0;
                CellInfo next_cell = Get_random_cell(boardInfo);
                bool stop_condition = false;
                float current_action;
                float reward;
                float next_Qmax, next_Q;
                float alpha = 0.7f;
                float gamma = 0.5f;
                while (!stop_condition)
                {
                    CellInfo current_cell = next_cell;
                    //Debug.Log("Estado aleatorio: " + current_cell.GetPosition);
                    current_action = Get_random_action(boardInfo, current_cell); //llamar a GetNextMove()
                    //Debug.Log("Accion aleatoria: " + current_action);
                    next_cell = Run_FSM(boardInfo, current_cell, current_action);
                    //Debug.Log("Realizar accion: " + next_cell.GetPosition);
                    float current_Q = Get_Q(current_cell, current_action, boardInfo);


                    reward = Get_Reward(current_cell, current_action, next_cell, goals);
                    //Debug.Log(reward);
                    next_Qmax = Get_maxQ(next_cell, boardInfo);
                    //Debug.Log(next_Qmax);
                    next_Q = Update_rule(current_Q, reward, next_Qmax, alpha, gamma);
                    //Debug.Log(next_Q);
                    /*tableQ =*/
                    Update_tableQ(current_cell, current_action, next_Q, boardInfo);

                    iter = iter + 1; //volvemos a empezar con nuevo valor aleatorio
                    stop_condition = Evaluate_stop(iter, N_ITER_MAX, next_cell, goals);
                    //Debug.Log(stop_condition);
                }
            }
            //Cuando termina todo escribimos resultado en fichero:
            copyResult(path);
        }

        public CellInfo Get_random_cell(BoardInfo board/*, CellInfo currentPos, CellInfo[] goals*/)
        {
            /*
            CellInfo celda;

            var valx = Random.Range(0, 14);
            var valy = Random.Range(0, 8);
            celda = new CellInfo(valx, valy);
            return celda;*/
            //test 2

            CellInfo randomCell = null;

            bool accion_valida = false;

            while (!accion_valida)
            {
                //Coordenadas aleatorias:
                int column = Random.Range(0, board.NumColumns);
                int row = Random.Range(0, board.NumRows);

                randomCell = board.CellInfos[column, row]; //buscamos en el tablero la celda aleatoria

                if (randomCell.Walkable) accion_valida = true;
            }

            return randomCell;
        }

        public float Get_random_action(BoardInfo board, CellInfo current_cell)
        {
            /*
            var val = Random.Range(0, 4);
            if (val == 0) return 1.0f; //n
            if (val == 1) return 2.0f; //e
            if (val == 2) return 3.0f; //s
            return 4.0f; //e*/
            //test 2
            //Mirar las acciones disponibles (si hay muros o limites)
            CellInfo[] acciones = current_cell.WalkableNeighbours(board);
            //[0]: up(N), [1]: right(E), [2]: down(O), [3]: left(S)

            int rand = -1;
            bool accion_valida = false;

            while(!accion_valida)
            {
                rand = Random.Range(0, 4);

                if (acciones[rand] != null) accion_valida = true;
            }

            //Debug.Log("Accion: "+ rand);

            return rand;
            

        }

        private CellInfo Run_FSM(BoardInfo board, CellInfo current_cell, float current_action)
        {
            int row = current_cell.RowId;
            int column = current_cell.ColumnId;

            CellInfo objetivo;

            switch (current_action)
            {
                case 0:
                    objetivo = board.CellInfos[column, row + 1];
                    break;
                case 1:
                    objetivo = board.CellInfos[column + 1, row];
                    break;
                case 2:
                    objetivo = board.CellInfos[column, row - 1];
                    break;
                case 3:
                    objetivo = board.CellInfos[column - 1, row];
                    break;
                default:
                    objetivo = null;
                    Debug.Log("Error: no se ha encontrado el vecino");
                    break;
            }
            return objetivo;
        }
        
        

        public float Get_Q(CellInfo current_cell, float current_action, BoardInfo boardInfo){
           // float idx = current_cell.GetPosition.y*4.0f+current_cell.GetPosition.x;
            float idx = current_cell.RowId * boardInfo.NumColumns + current_cell.ColumnId;
            return tableQ[(int)idx, (int)current_action];
        }
        
        public float Get_maxQ(CellInfo next_cell, BoardInfo boardInfo){
            
            int idx = next_cell.RowId * boardInfo.NumColumns + next_cell.ColumnId;
            float max = tableQ[idx, 0];

            for (int i = 0; i < 4; i++)
            {
                if(max < tableQ[idx, i]){
                    max = tableQ[(int)idx,i];
                }
            }
                //float maxQ = -1;
                //for(int i = 0; i <4; i++){
                //    if(tableQ[(int)idx, i] > maxQ){
                //        maxQ = tableQ[(int)idx,i];
                //    }
                //}
            return max;
                //Debug.Log(maxQ);

        }

        public float Update_rule(float current_Q, float reward, float next_Qmax, float alpha, float gamma){
            float valorQ; 
            valorQ = (1.0f - alpha) * current_Q + (alpha * (reward + gamma * next_Qmax));
            return valorQ;
            //Debug.Log(Q);
        }

        public void Update_tableQ(CellInfo current_cell, float current_action, float next_Q, BoardInfo boardInfo){
            float idx = current_cell.RowId * boardInfo.NumColumns + current_cell.ColumnId;
            tableQ[(int)idx, (int)current_action] = next_Q;
            
            Debug.Log(tableQ[(int)idx, (int)current_action]);
            /*
            for (int i = 0; i < boardInfo.NumColumns; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                   Debug.Log(tableQ[0, 0]);
                }
            }*/
        }
        
        public float Get_Reward(CellInfo current_cell,float current_action, CellInfo next_cell, CellInfo[] goals){
            float reward = 0;
            if(!next_cell.Walkable){
                reward = -1;
            }
            
            for(int i = 0; i <goals.Length; i++){
                if(next_cell == goals[i]){
                    reward = 100;
                }
            }
            return reward;

        }
        
        public bool Evaluate_stop(int iter, int N_ITER_MAX, CellInfo next_cell, CellInfo[] goals){
            if(iter == N_ITER_MAX || next_cell == goals[0]){
                return true;
            }else{
                return false;
            }
        }
        
        private void createTableQ(BoardInfo board, string path)
        {
            //string path = "./Assets/Scripts/Grupo22/Qlearning/tableQ.txt";

            //FileStream fs = File.Create(path);

            char[] chars = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N' };

            using (StreamWriter writeficheroQ = new StreamWriter(path))
            {
                //112 estados - 12 de muros
                //4 acciones: N, S, E, O
                for (int i = 0; i < board.NumRows; i++) //filas
                {
                    for (int j = 0; j < board.NumColumns; j++) //columnas
                    {
                        //Guardamos el tablero en orden desde la primera celda comenzando desde arriba a la izquierda hasta la ultima abajo a la derecha
                        //Las columnas se guardaran por letra de la A a la N, y las filas por numero del 1 al 8
                        writeficheroQ.Write(i);
                        writeficheroQ.Write(chars[j] + ":");
                        for (int k = 0; k < 4; k++) //N/ S/ E/ O
                        {
                            writeficheroQ.Write(0 + "|");
                        }
                        writeficheroQ.WriteLine("");

                    }
                }

                writeficheroQ.Close();
            }
        }

        private float[,] arrayTablaQ(string path)
        {
            float[,] array = new float[14 * 8, 4];
            //Variable para buscar valor
            string line = "Not found";
            int index = -1;
            float valorQ = -1;

            try
            {
                using (StreamReader buscar = new StreamReader(path))
                {
                    int linea = 0;
                    while (!buscar.EndOfStream)
                    {

                        line = buscar.ReadLine();
                        string[] celda = line.Split(':', '|');
                        //[0] = XY
                        //[1] = N
                        //[2] = E
                        //[3] = S
                        //[4] = O
                        //for (int i = 0; i < celda.Length-1; i++)
                        //{
                        //    Debug.Log(celda[i]);
                        //}

                        //Debug.Log(celda[0]);
                        for (int i = 1; i < celda.Length - 1; i++)
                        {
                            array[linea, i-1] = float.Parse(celda[i]);
                            //Debug.Log(celda[i]+"-->"+array[linea, i-1]);
                        }
                        linea++;
                        //Debug.Log("Comienza segunda ronda");

                    }

                    buscar.DiscardBufferedData();
                    buscar.BaseStream.Seek(0, SeekOrigin.Begin);

                    buscar.Close();
                }
            }
            catch (FileNotFoundException e)
            {
                Debug.Log("Error: Fichero no encontrado");
            }


            return array;
        }

        private Locomotion.MoveDirection explotar(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            //1.next_cell = Get_initial_cell();
            CellInfo next_cell = Get_initial_cell(currentPos);
            CellInfo current_cell = next_cell;
            //posicion incial
            //2.current_cell = next_cell; //pasar a
            //CellInfo next_cell;
            //3.decision_made = Get_best_action(tableQ, current_cell)

           int decision_made = Get_best_action(current_cell, boardInfo);

            for (int i = 0; i < 8 * 14; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Debug.Log(tableQ[i, j]);
                }
            }

            //Debug.Log(decision_made);

            if (decision_made == 0) return Locomotion.MoveDirection.Up;
            if (decision_made == 1) return Locomotion.MoveDirection.Right;
            if (decision_made == 2) return Locomotion.MoveDirection.Down;
            return Locomotion.MoveDirection.Left;
            //return decision_made
            //}
            //4.next_cell = Run_FSM(current_cell, decision_made); //
            /*
            * ยก Realmente, la FSM es el propio juego !
            */
            //5.go back to 2


        }

        CellInfo Get_initial_cell(CellInfo currentPos)
        {
            return currentPos;
        }

        public int Get_best_action(CellInfo current_cell, BoardInfo boardInfo)
        {
            float bestQ = float.NegativeInfinity;
            int action = -1;
            int idx = current_cell.RowId * boardInfo.NumColumns + current_cell.ColumnId;
            int best_action = action;

            for (int i = 0; i < 4; i++)
            {
                if (tableQ[(int)idx, i] > bestQ)
                {
                    bestQ = tableQ[idx, i];
                    best_action = i;
                }
                //Debug.Log("Best action" + bestQ);
            }
            //Debug.Log(current_cell.GetPosition);
            //Debug.Log(bestQ);
            return best_action;

        }
        

        //esto escribe array final fichero
        private void copyResult(string path)
        {

            char[] chars = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N' };

            using (StreamWriter sw = new StreamWriter(path, false))
            {
                //112 estados - 12 de muros
                //4 acciones: N, S, E, O
                int filas = 8;
                int columnas = 14;
                int acciones = 4;

                for (int f = 0; f < filas; f++)
                {
                    for (int c = 0; c < columnas; c++)
                    {
                        sw.Write(f);
                        sw.Write(chars[c] + ":");
                        for (int acc = 0; acc < acciones; acc++) //columnas
                        {
                            sw.Write(tableQ[f*c, acc] + "|");
                        }
                        sw.WriteLine("");
                    }
            
                }

                sw.Close();
            }
        }


    }

}