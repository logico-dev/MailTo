﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailTo.SRC
{
    public class State
    {
        private List<SmtpConfig> ServidoresDisponibles = new List<SmtpConfig>();

        private Stack<SmtpConfig> Servidores = new Stack<SmtpConfig>();
        private Queue<Sender> Mensajes = new Queue<Sender>();

        /// <summary>
        /// Estado del envío.
        /// 0=Detenido
        /// 1=Enviando
        /// 2=Pausado
        /// </summary>
        public int Estado = 0;
        public bool QueueLoaded = false;

        /// <summary>
        /// Agrega un servidor a la lista de serdidores
        /// </summary>
        /// <param name="server">Servidor SMTP a agregar</param>
        public void AddServer(SmtpConfig server)
        {
            ServidoresDisponibles.Add(server);
        }

        public void AddMessage(Sender messaje)
        {
            Mensajes.Enqueue(messaje);
            QueueLoaded = true;
        }

        private void Roulette()
        {
            if (ServidoresDisponibles.Count > 0 && !Servidores.Any())
            {
                List<SmtpConfig> servidoresSorteo = ServidoresDisponibles;
                while (servidoresSorteo.Count > 0)
                {
                    Random random = new Random();
                    int winner = random.Next(0, servidoresSorteo.Count);
                    Servidores.Push(servidoresSorteo[winner]);
                    servidoresSorteo.RemoveAt(winner);
                }
            }
        }

        public void SendNextMessage() {
            Roulette();
            if (Mensajes.Any()) {
                Estado = 1;
                Mensajes.Peek().LoadServer(Servidores.Pop());
                Mensajes.Dequeue().Send();
                if (!Mensajes.Any()) Estado = 0;
            } else
            {
                Estado = 0;
                QueueLoaded = false;
            }
        }
    }
}
