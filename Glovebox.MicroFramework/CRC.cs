using System;

namespace Glovebox.IoT {
    public class CRC {

        private const ushort P_16 = 0xA001;
        private const uint P_32 = 0xEDB88320;
        private const ushort P_CCITT = 0x1021;
        private const ushort P_DNP = 0xA6BC;

        static bool crc_tab16_init = false;
        static bool crc_tab32_init = false;
        static bool crc_tabccitt_init = false;
        static bool crc_tabdnp_init = false;

        static ushort[] crc_tab16 = new ushort[256];
        static uint[] crc_tab32 = new uint[256];
        static ushort[] crc_tabccitt = new ushort[256];
        static ushort[] crc_tabdnp = new ushort[256];

        public static ushort CRC16(string input) {
            ushort crc_16 = 0;

            for (int i = 0; i < input.Length; i++)
                crc_16 = update_crc_16(crc_16, input[i]);

            return crc_16;
        }

        public static ushort CRC16(byte[] input, int startPoint, int length) {
            ushort crc_16 = 0;

            for (int i = startPoint; i < length; i++)
                crc_16 = update_crc_16(crc_16, (char)input[i]);

            return crc_16;
        }

        public static uint CRC32(string input) {
            uint crc_32 = 0xffffffff;

            for (int i = 0; i < input.Length; i++)
                crc_32 = update_crc_32(crc_32, input[i]);

            crc_32 ^= 0xffffffff;

            return crc_32;
        }

        public static ushort CRC_CCITT_0000(string input) {
            ushort crc_ccitt_0000 = 0;

            for (int i = 0; i < input.Length; i++)
                crc_ccitt_0000 = update_crc_ccitt(crc_ccitt_0000, input[i]);

            return crc_ccitt_0000;
        }

        public static ushort CRC_CCITT_FFFF(string input) {
            ushort crc_ccitt_FFFF = 0xffff;

            for (int i = 0; i < input.Length; i++)
                crc_ccitt_FFFF = update_crc_ccitt(crc_ccitt_FFFF, input[i]);

            return crc_ccitt_FFFF;
        }

        public static ushort CRC_DNP(string input) {
            ushort crc_dnp = 0;
            ushort low_byte, high_byte;

            for (int i = 0; i < input.Length; i++)
                crc_dnp = update_crc_dnp(crc_dnp, input[i]);

            crc_dnp = (ushort)(~crc_dnp);
            low_byte = (ushort)((crc_dnp & 0xff00) >> 8);
            high_byte = (ushort)((crc_dnp & 0x00ff) << 8);
            crc_dnp = (ushort)(low_byte | high_byte);

            return crc_dnp;
        }


        private static ushort update_crc_ccitt(ushort crc, char c) {

            ushort tmp, short_c;

            short_c = (ushort)(0x00ff & c);

            if (!crc_tabccitt_init) init_crcccitt_tab();

            tmp = (ushort)((crc >> 8) ^ short_c);
            crc = (ushort)((crc << 8) ^ crc_tabccitt[tmp]);

            return crc;

        }  /* update_crc_ccitt */

        private static ushort update_crc_16(ushort crc, char c) {

            ushort tmp, short_c;

            short_c = (ushort)(0x00ff & c);

            if (!crc_tab16_init) init_crc16_tab();

            tmp = (ushort)(crc ^ short_c);
            crc = (ushort)((crc >> 8) ^ crc_tab16[tmp & 0xff]);

            return crc;

        }  /* update_crc_16 */

        private static ushort update_crc_dnp(ushort crc, char c) {

            ushort tmp, short_c;

            short_c = (ushort)(0x00ff & c);

            if (!crc_tabdnp_init) init_crcdnp_tab();

            tmp = (ushort)(crc ^ short_c);
            crc = (ushort)((crc >> 8) ^ crc_tabdnp[tmp & 0xff]);

            return crc;

        }  /* update_crc_dnp */

        private static uint update_crc_32(uint crc, char c) {

            uint tmp, long_c;

            long_c = 0x000000ff & (uint)c;

            if (!crc_tab32_init) init_crc32_tab();

            tmp = crc ^ long_c;
            crc = (crc >> 8) ^ crc_tab32[tmp & 0xff];

            return crc;

        }  /* update_crc_32 */


        private static void init_crc16_tab() {

            int i, j;
            ushort crc, c;

            for (i = 0; i < 256; i++) {

                crc = 0;
                c = (ushort)i;

                for (j = 0; j < 8; j++) {

                    if (((crc ^ c) & 0x0001) != 0)
                        crc = (ushort)((crc >> 1) ^ P_16);
                    else
                        crc = (ushort)(crc >> 1);

                    c = (ushort)(c >> 1);
                }

                crc_tab16[i] = crc;
            }

            crc_tab16_init = true;

        }  /* init_crc16_tab */

        private static void init_crcdnp_tab() {

            int i, j;
            ushort crc, c;

            for (i = 0; i < 256; i++) {

                crc = 0;
                c = (ushort)i;

                for (j = 0; j < 8; j++) {

                    if (((crc ^ c) & 0x0001) != 0)
                        crc = (ushort)((crc >> 1) ^ P_DNP);
                    else
                        crc = (ushort)(crc >> 1);

                    c = (ushort)(c >> 1);
                }

                crc_tabdnp[i] = crc;
            }

            crc_tabdnp_init = true;

        }  /* init_crcdnp_tab */

        private static void init_crc32_tab() {

            int i, j;
            uint crc;

            for (i = 0; i < 256; i++) {

                crc = (uint)i;

                for (j = 0; j < 8; j++) {

                    if ((crc & 0x00000001u) != 0)
                        crc = (crc >> 1) ^ P_32;
                    else
                        crc = crc >> 1;
                }

                crc_tab32[i] = crc;
            }

            crc_tab32_init = true;

        }  /* init_crc32_tab */

        private static void init_crcccitt_tab() {

            int i, j;
            ushort crc, c;

            for (i = 0; i < 256; i++) {

                crc = 0;
                c = (ushort)(i << 8);

                for (j = 0; j < 8; j++) {

                    if (((crc ^ c) & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ P_CCITT);
                    else
                        crc = (ushort)(crc << 1);

                    c = (ushort)(c << 1);
                }

                crc_tabccitt[i] = crc;
            }

            crc_tabccitt_init = true;

        }  /* init_crcccitt_tab */

    }
}
