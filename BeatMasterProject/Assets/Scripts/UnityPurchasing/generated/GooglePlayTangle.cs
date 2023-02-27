// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("AEoQIy/KGI4Hx7t4jQTtMFUkHBekPN9DkHA6NxCaffTEiauAjWrS5GtsWgS+VcHFC0P7IDxRXvORDq1/M0INT+hZDzYE/CFx93U+8E4oVuyw/IjzvPN3kkSHALLBJHPcwTaCignr0AFkyGqHzuHf5embPiBBthM3+fpH2yJTpHZGsz81hWEMbrgWU7oWmsI1KMtA2U0/GWhQlgZUkVa+z4atvI5pFE43l0EOeboCCvJ4L39fjwwCDT2PDAcPjwwMDbhQDTPxPZWBdU8JodVe+/EnSAbs3neZUsqgia2POcPtS320VoXrxu6GR4wkpjvJPY8MLz0ACwQni0WL+gAMDAwIDQ6IGhHBI4u3VoCSHSySB2UpJ9WlLcG3SuEeBQyJqg8ODA0M");
        private static int[] order = new int[] { 5,7,11,9,9,11,13,13,8,13,13,13,13,13,14 };
        private static int key = 13;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
