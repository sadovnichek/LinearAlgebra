using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LinearAlgebra
{
    public class LinearManifold : Vector
    {
        private Vector ShiftVector;

        private Vector[] SubspaceBasis;

        public LinearManifold(Vector shiftVector, Vector[] subspaceBasis)
        {
            ShiftVector = shiftVector;
            SubspaceBasis = subspaceBasis;
        }

        public List<Vector> GetSubspaceBasis()
        {
            return SubspaceBasis.ToList();
        }

        public Vector GetShiftVector()
        {
            return ShiftVector;
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
