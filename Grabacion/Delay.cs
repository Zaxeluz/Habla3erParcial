using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;


namespace Grabacion
{
    class Papa
    {
        private ISampleProvider fuente;

        public int offsetTiempoMS;

        List<float> muestras = new List<float>();

        private int tamañoBuffer;
        private int cantidadMuestasBorradas = 0;
        private int cantidadMuestrasTranscurridas = 0;

        public Papa(ISampleProvider fuente)
        {
            this.fuente = fuente;
            offsetTiempoMS = 600;

            tamañoBuffer = 20 * fuente.WaveFormat.SampleRate;

        }

        public WaveFormat WaveFormat
        {
            get
            {
                return fuente.WaveFormat;
            }
        }


        //Offset es el numero de muestras leídas hasta ahorita
        public int Read(float[] buffer, int offset, int count)
        {
            //Calculo de tiempos
            var read = fuente.Read(buffer, offset, count);
            float tiempoTranscurrido =
               (float)cantidadMuestrasTranscurridas / (float)fuente.WaveFormat.SampleRate;

            float tiempoTranscurridoMS = tiempoTranscurrido * 1000;
            int numMuestrasOffsetTiempo = (int)
                (((float)offsetTiempoMS / 1000.0f) * (float)fuente.WaveFormat.SampleRate);

            //Añadir muestras a nuestro buffer
            for (int i = 0; i < read; i++)
            {
                muestras.Add(buffer[i]);
            }
            //Quitar elementos de nuestro buffer si se pasó del máximo
            if (muestras.Count > tamañoBuffer)
            {
                //Ya se pasó
                int diferencia = muestras.Count - tamañoBuffer;
                muestras.RemoveRange(0, diferencia);
                cantidadMuestasBorradas += diferencia;
            }

            //Modificar muestras
            if (tiempoTranscurridoMS > offsetTiempoMS)
            {
                for (int i = 0; i < read; i++)
                {

                    buffer[i] +=
                        muestras[(cantidadMuestrasTranscurridas - cantidadMuestasBorradas) +
                        i - numMuestrasOffsetTiempo];
                }
            }

            cantidadMuestrasTranscurridas += read;
            return read;
        }
    }
}
