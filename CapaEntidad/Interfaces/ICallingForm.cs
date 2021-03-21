using CapaEntidad.Entidades.Cuentas;

namespace CapaEntidad.Interfaces
{
    public interface ICallingForm
    {
        /// <summary>
        /// Esta firma se implementa en todos los formularios que requieran 
        /// abrir la lista de cuentas para seleccionar una para ser utilizada desde 
        /// el formulario que llama a la lista (el tree view)
        /// </summary>
        /// <param name="cuenta">Cuenta a tranferir</param>
        /// <returns>Si el calling form la recibio correctamente, usaer en caso 
        /// por ejemplo que solo podamos recibir cuentas auxiliares
        /// </returns>
        bool TransferirCuenta(Cuenta cuenta);
        
    }
}
