using AriesContador.Core.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.UnitTest
{
    public class HashObjectsTest
    {

        [Fact]
        public void DeepClone_HashOfObjectShouldDiffer()
        {
            // Crear una instancia del objeto y clonarlo
            var originalObject = new SampleClass { Property1 = 1, Property2 = "Test" };
            var clonedObject = originalObject.DeepClone();

            // Obtener los hashes de ambos objetos
            int hashOriginal = originalObject.GetHashCode();
            int hashCloned = clonedObject.GetHashCode();

            // Comparar los hashes
            Assert.NotEqual(hashOriginal, hashCloned);
        }
    }
    public class SampleClass
    {
        public int Property1 { get; set; }
        public string Property2 { get; set; }
    }
}
