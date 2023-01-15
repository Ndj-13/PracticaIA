using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.DataStructures;
using System.Linq;
using UnityEngine;
using System.IO;

namespace Assets.Scripts.Grupo22.Qlearning
{
    public class QlearningMind : AbstractPathMind
    {
        public FileStream tablaQ;
        public override void Repath()
        {
     
        }

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            //1º Crear fichero
            if (!File.Exists("./Assets/Scripts/Grupo22/Qlearning/tableQ.txt"))
            {
                tablaQ = createTableQ(boardInfo);
            } else
            {
                tablaQ = new FileStream("./Assets/Scripts/Grupo22/Qlearning/tableQ.txt", FileMode.Open, FileAccess.Read);
            }
            

            //2º Exploracion 100%
            exploracionQlearning(boardInfo, tablaQ);

            //3º Explotacion (tablaQ ya aprendida)
            return Locomotion.MoveDirection.None; 

        }

        private FileStream createTableQ(BoardInfo board)
        {
            string path = "./Assets/Scripts/Grupo22/Qlearning/tableQ.txt";

            FileStream fs = File.Create(path);

            char[] chars = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N'};

            using (StreamWriter sw = new StreamWriter(fs))
            {
                //112 estados - 12 de muros
                //4 acciones: N, S, E, O
                for (int i = 0; i < board.NumRows; i++) //filas
                {
                    for (int j = 0; j < board.NumColumns; j++) //columnas
                    {
                        //Guardamos el tablero en orden desde la primera celda comenzando desde arriba a la izquierda hasta la ultima abajo a la derecha
                        //Las columnas se guardaran por letra de la A a la N, y las filas por numero del 1 al 8
                        sw.Write(i);
                        sw.Write(chars[j] + ":");
                        for (int k = 0; k < 4; k++) //N/ S/ E/ O
                        {
                            sw.Write(0 + "|");
                        }
                        sw.WriteLine("");

                    }
                }

                sw.Close();
            }
            return fs;
        }

        private void exploracionQlearning(BoardInfo board, FileStream tablaQ)
        {
            //Episodios: 
            int N_EPI_MAX = 10;
            //Celdas que recorrer:
            int N_ITER_MAX = 100; //112-12 q son muros

            for (int k = 0; k < N_EPI_MAX; k++)
            {
                int iter = 0; //se ira sumando hasta N_ITER_MAX
                CellInfo next_cell = Get_random_cell(board); //generamos celda aleatoria para empezar
                bool stop_condition = false;
                while (!stop_condition)
                {
                    //Estado y accion aleatorios
                    CellInfo current_cell = next_cell; //estado generado aleatoriamente
                    Debug.Log("Estado aleatorio: " + current_cell.GetPosition);
                    int current_action = Get_random_action(board, current_cell); //accion aleatoria (N, S, E, O)
                    Debug.Log("Accion aleatoria: " + current_action);
                    next_cell = Run_FSM(board, current_cell, current_action); //sabiendo celda aleatoria en la que estamos y la accion que nos ha tocado, la celda vecina a la que toca acceder
                    Debug.Log("Realizar accion: "+ next_cell.GetPosition);

                    //Calcular Q
                    float current_Q = Get_Q(tablaQ, current_cell, current_action); //coger valor Q celda actual
                    Debug.Log(current_Q);
                    bool reward = Get_Reward(board, next_cell); //mirar si celda a la que va a acceder es recompensa
                    next_Qmax = Get_maxQ(tableQ, next_cell); //miramos cual es el valor +alto q hay de Q en la siguiente habitacion
                    //
                    //next_Q = Update_rule(current_Q, reward, next_Qmax, alpha, gamma); //calculamos nuevo valor de Q
                    //tableQ = Update_tableQ(tableQ, current_cell, current_action, next_Q); //lo actualizamos en la tabla

                    iter = iter + 1; //volvemos a empezar con nuevo valor aleatorio
                    stop_condition = true;

         //stop_condition = Evaluate_stop(iter, N_ITER_MAX, next_cell);
                }
            }
        }

        //Estado aleatorio
        private CellInfo Get_random_cell(BoardInfo board) 
        {
            CellInfo randomCell;

            //Coordenadas aleatorias:
            int column = Random.Range(0, board.NumColumns);
            int row = Random.Range(0, board.NumRows);

            randomCell = board.CellInfos[column, row]; //buscamos en el tablero la celda aleatoria

            return randomCell;
        }

        //Accion aleatoria
        private int Get_random_action(BoardInfo board, CellInfo current_cell)
        {
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

        //Buscamos celda a la que tenemos que ir
        private CellInfo Run_FSM(BoardInfo board, CellInfo current_cell, int current_action)
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

        //Valor Q de la celda actual
        private float Get_Q(FileStream tablaQ, CellInfo current_cell, int current_action)
        {
            //Variable para buscar valor
            string line = "Not found";
            float valorQ = -1;

            try
            {
                using (StreamReader buscar = new StreamReader(tablaQ))
                {
                    //Primero buscar en que fila esta
                    int fila = 0;
                    int columna = 0;
                    int accion = 0;
                    
                    while (!buscar.EndOfStream && fila <= current_cell.RowId*14)
                    {
                        int i = 0;
                        while (i < 14)
                        {
                            line = buscar.ReadLine();
                            i++;
                        }
                        fila += 14;
                    }
                    //Ya hemos llegado a la fila, falta la columna
                    while(!buscar.EndOfStream && columna <= current_cell.ColumnId)
                    {
                        line = buscar.ReadLine();
                        columna++;
                    }
                    Debug.Log(line);

                    //Ya estamos en la celda, ahora ir a N, S, E, O
                    //Primero creamos array con cada valor
                    string[] celda = line.Split(':', '|');
                    //[0] = XY
                    //[1] = 0 (N)
                    //[2] = 1 (E)
                    //[3] = 2 (S)
                    //[4] = 3 (O)

                    while(accion <= current_action+1)
                    {
                        accion++;
                    }
                    valorQ = float.Parse(celda[accion]);

                    buscar.DiscardBufferedData();
                    buscar.BaseStream.Seek(0, SeekOrigin.Begin);
                }
             
            }
            catch(FileNotFoundException e)
            {
                Debug.Log("Error: Fichero no encontrado");
            }
            
            return valorQ;
     
        }

        private bool Get_Reward(BoardInfo board, CellInfo next_cell)
        {
            if(next_cell == board.Exit)
            {
                return true;
            } else
            {
                return false;
            }
        }

        private char letraColumna(int num)
        {
            switch(num)
            {
                case 0:
                    return 'A';
                case 1:
                    return 'B';
                case 2:
                    return 'C';
                case 3:
                    return 'D';
                case 4:
                    return 'E';
                case 5:
                    return 'F';
                case 6:
                    return 'G';
                case 7:
                    return 'H';
                case 8:
                    return 'I';
                case 9:
                    return 'J';
                case 10:
                    return 'K';
                case 11:
                    return 'L';
                case 12:
                    return 'M';
                case 13:
                    return 'N';
                default:
                    return ' ';

            }
        }
    }
}
