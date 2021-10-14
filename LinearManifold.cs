using System.Globalization;
using System.Linq;

namespace LinearAlgebra
{
    public class LinearManifold : Vector
    {
        public Vector ShiftVector;

        public Vector[] SubspaceBasis;

        public LinearManifold(Vector shiftVector, Vector[] subspaceBasis)
        {
            ShiftVector = shiftVector;
            SubspaceBasis = subspaceBasis;
        }

        public override string ToString()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            var shiftVector = ShiftVector.ToString();
            var subspaceBasis = "<" + string.Join(",", SubspaceBasis.Select(v => v.ToString())) + ">";
            if (shiftVector.All(x => x == 0))
                return subspaceBasis;
            else
                return shiftVector + " + " + subspaceBasis;
        }
    }
}
