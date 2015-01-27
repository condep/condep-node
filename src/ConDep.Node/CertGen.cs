using Org.BouncyCastle.Asn1;

namespace ConDep.Node
{
    public class CertGen
    {
        /*
E = condep@con-dep.net
CN = node.con-dep.net
O = ConDep
L = Bergen
S = Hordaland
C = NO
        */
        public void GenerateCert()
        {
        }
    }

    public enum CertStrength
    {
        bits_512 = 512, bits_1024 = 1024, bits_2048 = 2048, bits_4096 = 4096
    }
}