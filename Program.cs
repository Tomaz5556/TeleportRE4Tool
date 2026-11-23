using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TeleportTrainer
{
    public class Option
    {
        public string Label;
        public byte[] Values;
        public Option(string label, string hex)
        {
            Label = label;
            string[] parts = hex.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Values = new byte[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                Values[i] = Convert.ToByte(parts[i], 16);
        }
    }

    public class MainForm : Form
    {
        private ListBox listBox;
        private TextBox txtAddress;
        private Button btnTeleport;
        private Button btnScan;
        private Label lblStatus;
        private Label lblCount;

        private static readonly List<Option> Options = new List<Option>
        {
            new Option("r101 >> r100", "D8 85 BC 47 64 99 D3 C5 36 61 05 C6 86 EF 64 BF 01 00"),
            new Option("r100 >> r101", "67 B0 4B C7 40 24 B5 42 8C F8 AF 46 34 CF 07 40 01 01"),
            new Option("r102 >> r101", "CF 2B 91 46 70 7C 70 44 DF E8 4F 45 CB 70 DF BF 01 01"),
            new Option("r103 >> r101", "AF CB 19 47 93 17 48 45 11 55 D0 C6 CF 7C C0 BF 01 01"),
            new Option("r105 >> r101", "14 7B 5C C6 B0 8E 29 45 C0 6A 03 47 74 1A 33 40 01 01"),
            new Option("r101 >> r102", "D8 D6 99 C6 00 00 83 BB 9F 1F 95 C6 99 E8 7C 3F 01 02"),
            new Option("r108 >> r102", "D7 8F 9B C5 00 80 04 C6 CE 35 4D 47 1E 52 1E C0 01 02"),
            new Option("r101 >> r103", "0F F9 39 C7 60 46 3D 41 7D 5C DD 45 8E CA F1 3F 01 03"),
            new Option("r106 >> r103", "10 F6 13 46 68 97 17 42 81 B7 3D C4 8B 3F 13 C0 01 03"),
            new Option("r106 >> r104", "A4 29 57 C5 00 00 5E BB 4A C9 40 46 DB 4E CA BF 01 04"),
            new Option("r107 >> r104", "E6 A7 D7 46 0C 17 90 C2 82 9E D8 C6 A2 C8 91 BF 01 04"),
            new Option("r101 >> r105", "35 25 8E 47 AF DA 0B C5 75 59 12 C7 3A F1 9C BF 01 05"),
            new Option("r107 >> r105", "BE 0F 01 C7 00 00 83 BB 73 F4 DF C4 FE D0 BF 3F 01 05"),
            new Option("r103 >> r106", "05 14 2E 46 06 42 5F C4 42 17 F0 C2 9B DB 56 3F 01 06"),
            new Option("r104 >> r107", "02 E6 E7 46 C0 63 4A C1 C7 C0 DE C6 EB 51 12 40 01 07"),
            new Option("r105 >> r107", "9D 02 B5 47 89 7A A1 C5 55 83 3B C7 AE 6A DF BF 01 07"),
            new Option("r102 >> r108", "9D 80 2B C7 58 9A 16 C2 73 45 66 C6 D7 A3 00 40 01 08"),
            new Option("r109 >> r108", "06 C5 B8 47 AC EF 0C 45 1F AB 4A 46 50 F3 F7 BF 01 08"),
            new Option("r108 >> r109", "31 EF BD 47 79 B6 0C 45 C2 21 50 46 14 06 80 3F 01 09"),
            new Option("r10a >> r109", "C1 8D 00 48 C5 D3 12 45 F7 E1 2D 46 10 0B DD BF 01 09"),
            new Option("r10e >> r109", "B5 73 FE 47 CC E0 0B 44 C0 92 74 46 70 B5 43 C0 01 09"),
            new Option("r109 >> r10a", "E6 D6 63 C7 C5 8C E5 45 F2 4C 88 47 70 95 B1 3F 01 0A"),
            new Option("r10b >> r10a", "1D 9A CA 47 8A E3 92 C5 2A 78 5E 47 90 C7 C1 BF 01 0A"),
            new Option("r10a >> r10b", "A5 A0 8D C7 6F A6 34 45 11 65 B8 C6 B2 45 CD 3F 01 0B"),
            new Option("r10e >> r10c", "01 B2 D9 46 B5 4E 6F C6 C0 9E 2B 47 8A BA 1A 40 01 0C"),
            new Option("r11b >> r10c", "D7 AD 52 C4 00 00 C6 3B CF 38 17 C4 F0 D0 B1 3B 01 0C"),
            new Option("r11b >> r10d", "2E 7A 20 43 00 A0 1B C5 D3 3F 03 C5 99 01 46 40 01 0D"),
            new Option("r109 >> r10e", "1D 3D 22 C7 B0 84 B3 C5 B5 EB 2D 46 50 2E B7 BD 01 0E"),
            new Option("r10c >> r10e", "31 14 13 47 57 F9 62 C6 39 53 16 47 C6 92 B8 BE 01 0E"),
            new Option("r119 >> r10e", "50 88 22 C7 B4 84 B3 C5 E7 94 2B 46 40 28 FD BC 01 0E"),
            new Option("r11d >> r10f", "A5 59 C4 C6 16 00 7A 45 9E 1D 80 46 AA EC AC 3F 01 0F"),
            new Option("r11e >> r10f", "CA 6C B1 C6 00 00 B2 3B B3 57 12 47 B6 98 EE 3F 01 0F"),
            new Option("r11f >> r10f", "D0 87 5B 47 3E 0C F4 C6 A9 DB 2C C8 02 CE B8 BF 01 0F"),
            new Option("r200 >> r10f", "E4 EE 38 46 DA F0 79 45 D8 67 67 46 87 6F DE BF 01 0F"),
            new Option("r112 >> r111", "EB 1A 96 46 E1 FF 71 44 92 2A 3C 45 2A 3E 11 C0 01 11"),
            new Option("r113 >> r111", "AF CB 19 47 93 17 48 45 11 55 D0 C6 CF 7C C0 BF 01 11"),
            new Option("r111 >> r112", "D8 D6 99 C6 00 00 83 BB 9F 1F 95 C6 99 E8 7C 3F 01 12"),
            new Option("r118 >> r112", "E3 08 A1 C5 74 68 04 C6 53 56 4B 47 30 5B 1D C0 01 12"),
            new Option("r111 >> r113", "0F F9 39 C7 60 46 3D 41 7D 5C DD 45 8E CA F1 3F 01 13"),
            new Option("r11c >> r113", "27 86 B4 44 F6 20 12 44 3F C5 DA C6 9A 23 C5 BF 01 13"),
            new Option("r108 >> r117", "42 2D 2A 46 40 B1 A4 C4 2B 97 D1 45 E2 E4 3B 40 01 17"),
            new Option("r118 >> r117", "19 2E 2A 46 87 62 A4 C4 94 62 D1 45 C2 7B 47 C0 01 17"),
            new Option("r118 >> r117", "05 F2 28 43 19 00 7A C4 CE C4 BD 45 88 06 3B 40 01 17"),
            new Option("r112 >> r118", "32 6D 2D C7 28 64 1D 43 1E 4D 61 C6 7C 5D 09 40 01 18"),
            new Option("r117 >> r118", "DC E7 B6 46 C4 E6 3B 46 98 E3 CE C6 F4 08 B6 BF 01 18"),
            new Option("r117 >> r118", "FF 83 E3 46 98 7A 2A 46 24 70 81 C6 C9 28 CF BF 01 18"),
            new Option("r119 >> r118", "FF AC B8 47 6D 1E 0D 45 20 E8 4A 46 6C C1 EB BF 01 18"),
            new Option("r10e >> r119", "B5 73 FE 47 CC E0 0B 44 C0 92 74 46 70 B5 43 C0 01 19"),
            new Option("r118 >> r119", "31 EF BD 47 79 B6 0C 45 C2 21 50 46 14 06 80 3F 01 19"),
            new Option("r11a >> r119", "47 73 00 48 E7 A3 0F 45 27 2A 31 46 3D 2E F7 BF 01 19"),
            new Option("r119 >> r11a", "EB AF 62 C7 C3 8C E5 45 8E 4B 88 47 BC 5A AE 3F 01 1A"),
            new Option("r11b >> r11a", "75 46 CB 47 8E E3 92 C5 F0 BB 5F 47 32 2A DF BF 01 1A"),
            new Option("r10b >> r11b", "13 A1 F5 47 42 01 2F C3 41 ED 27 48 2A 5E C0 3F 01 1B"),
            new Option("r10c >> r11b", "4C 1E 16 48 E4 54 86 42 A3 E6 2D 48 4D BA 27 C0 01 1B"),
            new Option("r10d >> r11b", "0B 82 09 48 1C 12 1D C4 D3 FD 56 C7 66 E3 43 BF 01 1B"),
            new Option("r11a >> r11b", "F8 00 8D C7 71 5C 39 45 69 6E B8 C6 A5 66 E1 3F 01 1B"),
            new Option("r113 >> r11c", "20 B3 49 47 00 00 26 3F 51 09 8A C7 11 44 61 3F 01 1C"),
            new Option("r11d >> r11c", "64 9E F6 47 B4 F6 2E C5 E3 A2 0C C7 AB 57 77 BF 01 1C"),
            new Option("r11e >> r11c", "44 EE CC 47 6A 80 A7 C4 1B 9F 9C C6 86 A7 39 40 01 1C"),
            new Option("r10f >> r11d", "65 EB 9D 46 00 00 0C BB DD FE 59 46 B2 8D E8 BF 01 1D"),
            new Option("r11c >> r11d", "66 53 82 C6 00 00 0C BB EA B4 44 C6 44 B3 C1 3F 01 1D"),
            new Option("r10f >> r11e", "F6 1D 77 47 E0 13 CD C1 A0 EB DF 46 9B C2 90 BF 01 1E"),
            new Option("r11c >> r11e", "47 41 B7 43 D0 25 CD C1 6B 43 7A C3 0C 01 1A BE 01 1E"),
            new Option("r10f >> r11f", "00 40 F1 45 00 00 7F C4 00 F0 25 C6 DE 7E BC 3F 01 1F"),
            new Option("r10f >> r200", "E6 DB BC C2 E0 9B 10 41 FA F0 43 C3 E4 A8 B6 3F 02 00"),
            new Option("r200 >> r200", "C1 93 A1 47 53 18 1E 46 40 58 49 C7 10 85 3B 40 02 00"),
            new Option("r202 >> r200", "C1 64 B1 47 22 FC 77 46 3B 67 86 C7 0E 7D DD BF 02 00"),
            new Option("r203 >> r201", "6C 14 FB C2 00 00 8E BB 7A 49 DF 44 50 57 3F 40 02 01"),
            new Option("r208 >> r201", "8C 83 21 47 EA FF F9 44 29 4D 79 C7 D8 E5 F5 BC 02 01"),
            new Option("r210 >> r201", "49 05 08 C6 E6 FF F9 44 8D 5D EB C6 E7 B6 AB 3F 02 01"),
            new Option("r219 >> r201", "13 77 92 43 6A FB 79 45 F0 7A 45 C7 1C BB 6E BE 02 01"),
            new Option("r200 >> r202", "2F 69 A2 C5 00 00 C8 BA 13 43 9A C3 0C A8 B9 3F 02 02"),
            new Option("r207 >> r202", "53 12 9F 46 FB 9F 0C 46 26 CD 62 C7 6E 29 D1 BE 02 02"),
            new Option("r201 >> r203", "93 20 F0 C6 17 DE 81 45 27 B7 B3 C6 00 A3 3D BE 02 03"),
            new Option("r207 >> r203", "7F 6A 36 C6 07 01 84 45 CF DE 21 C6 00 67 A8 BF 02 03"),
            new Option("r205 >> r204", "5A 7F 15 46 FD 7F BB 45 66 07 65 C5 C4 DC DF BF 02 04"),
            new Option("r205 >> r204", "A5 71 CD 44 00 00 A8 3B 1D 1A 32 C7 02 A4 DE BF 02 04"),
            new Option("r208 >> r204", "F4 B1 06 C3 00 00 F0 BA AE BE AE C4 E0 90 3B 40 02 04"),
            new Option("r209 >> r204", "00 40 1C C6 00 80 BB 45 00 D0 DE C6 9F 5A B9 3F 02 04"),
            new Option("r22c >> r204", "DA EF 05 C6 00 00 5E BB D6 ED E8 C6 50 48 0C BE 02 04"),
            new Option("r204 >> r205", "BE 65 7F C4 49 BD 79 45 A9 F5 2E 44 DD 1D AE 3F 02 05"),
            new Option("r204 >> r205", "82 31 F3 45 51 2F 1C 46 A7 35 28 47 C8 D9 D6 3F 02 05"),
            new Option("r20d >> r206", "35 5F 1C 46 F8 FF F9 45 12 E9 83 C5 90 51 E0 BF 02 06"),
            new Option("r20f >> r206", "D6 43 27 C6 08 00 FA 45 27 83 81 C5 80 61 C0 3F 02 06"),
            new Option("r210 >> r206", "48 EF 87 42 04 00 FA 45 46 A8 17 C7 08 0E 36 BE 02 06"),
            new Option("r202 >> r207", "3D 06 02 44 30 7F FD 42 15 6F 43 C2 17 14 37 C0 02 07"),
            new Option("r203 >> r207", "E7 8E 18 C6 57 A9 81 45 42 EC 26 C6 F4 D6 8C 3F 02 07"),
            new Option("r201 >> r208", "61 3A 35 BE F8 FF 79 45 80 20 94 C4 3E 31 3A 40 02 08"),
            new Option("r204 >> r208", "23 18 6C C0 05 E0 2B 46 80 64 CE C7 EC CE E9 3C 02 08"),
            new Option("r204 >> r209", "3E 82 0F 44 00 00 BC 3B DD ED F0 42 4C E5 B0 BF 02 09"),
            new Option("r20a >> r209", "8E A9 9C C7 F1 1F E1 45 B4 69 56 45 50 7F B6 3D 02 09"),
            new Option("r209 >> r20a", "ED 65 F8 43 C0 35 39 41 45 F6 CF 42 F6 E6 3D 40 02 0A"),
            new Option("r20b >> r20a", "16 8E 13 C7 21 2A F7 44 77 FE A1 C7 00 D9 CE BC 02 0A"),
            new Option("r20c >> r20a", "20 B9 3F 46 80 BB 41 C0 C2 9D 06 C7 38 9E 24 BE 02 0A"),
            new Option("r20a >> r20b", "7C F7 D2 C3 9B EF BC 45 E2 A2 70 44 80 7A 41 40 02 0B"),
            new Option("r20c >> r20b", "B9 EE EA 46 F8 FF 79 45 4D 0F 42 46 34 5A 45 40 02 0B"),
            new Option("r20a >> r20c", "0A AA 9F 46 FA BF 5A 45 01 04 0D 47 44 F0 44 C0 02 0C"),
            new Option("r20b >> r20c", "9F B3 47 C2 F5 BF 5A 45 AD EB 13 C4 24 A1 96 3E 02 0C"),
            new Option("r20f >> r20c", "92 B4 19 47 F4 BF 5A 45 21 E4 26 46 14 47 9E BF 02 0C"),
            new Option("r206 >> r20d", "FE 65 D1 C6 00 00 BC 3B 91 60 24 43 C0 4E EB 3F 02 0D"),
            new Option("r206 >> r20d", "37 09 D1 C6 00 00 A8 3B 27 1B DB 43 82 08 11 40 02 0D"),
            new Option("r20e >> r20d", "7F F6 52 46 00 00 20 BB 59 6D EE 46 84 01 46 C0 02 0D"),
            new Option("r20d >> r20e", "41 EB 75 46 00 00 A8 3B 74 75 ED 46 61 EC F3 3F 02 0E"),
            new Option("r206 >> r20f", "19 47 AB 47 F8 DF AB 45 D5 9D 2B 47 86 A5 CC BF 02 0F"),
            new Option("r20c >> r20f", "9A 3B 21 47 F7 BF 5A 45 0D E3 CC 46 28 F2 BB 3F 02 0F"),
            new Option("r201 >> r210", "39 19 4E C7 1A 00 FA C4 20 F8 6E 48 5B DC C1 BF 02 10"),
            new Option("r206 >> r210", "79 15 00 C1 00 00 5E BB 17 7C 5A C4 68 A3 3E 40 02 10"),
            new Option("r210 >> r210", "8A CF 6E C7 24 00 FA C4 4E AA 71 48 D7 9D D0 3F 02 10"),
            new Option("r210 >> r210", "51 A6 95 C6 0B 00 FA C4 B6 13 EC C5 D6 9B EB 3F 02 10"),
            new Option("r222 >> r210", "3E B0 DB 43 B8 FF 15 44 58 6F 31 C7 A8 88 5F BD 02 10"),
            new Option("r212 >> r211", "00 00 91 44 00 00 00 00 00 38 9A 46 DA A0 D5 3F 02 11"),
            new Option("r213 >> r211", "62 DB 12 46 00 10 96 BD FF 48 A0 C6 37 B3 F3 BD 02 11"),
            new Option("r216 >> r211", "1B A6 FE 46 00 00 89 BD FA 29 99 46 76 ED CC BF 02 11"),
            new Option("r219 >> r211", "B1 30 A3 45 00 D0 96 BD 6B A5 B3 46 A8 A8 3C 40 02 11"),
            new Option("r22c >> r211", "7D 80 5F 46 00 50 87 BD 19 24 DE 46 24 47 E5 BF 02 11"),
            new Option("r211 >> r212", "00 C0 7D 45 00 00 00 00 00 80 F7 44 DC 0F C9 BF 02 12"),
            new Option("r211 >> r213", "15 48 F2 C2 5E 00 7A 44 EA 57 D8 42 12 B7 46 40 02 13"),
            new Option("r214 >> r213", "38 83 93 C6 00 00 36 BB 79 DA 4F C7 DC 1F B6 3F 02 13"),
            new Option("r213 >> r214", "0F E8 08 46 5D 24 BC 44 3A 6B 18 47 D6 35 C8 BF 02 14"),
            new Option("r217 >> r214", "DB C2 3C C5 D0 A8 FA 45 53 C5 A6 C6 0C 33 C0 3F 02 14"),
            new Option("r217 >> r214", "4F F0 06 C4 40 2D A4 40 79 17 AD C6 C0 43 AC 3F 02 14"),
            new Option("r218 >> r214", "8D 39 E6 46 40 84 A4 40 79 9C AF C6 8D 7A D6 BF 02 14"),
            new Option("r218 >> r215", "88 8A CD 46 04 E5 38 45 E5 C4 49 41 A6 5B B3 3F 02 15"),
            new Option("r211 >> r216", "9C 88 89 C3 00 00 72 BB 16 6D 72 41 CC 59 DF 3F 02 16"),
            new Option("r214 >> r217", "AA 6F A0 45 D0 A8 FA 45 D1 12 80 C4 1E E4 E3 BF 02 17"),
            new Option("r214 >> r217", "A1 C8 FB 45 80 4C A4 40 04 F9 66 C4 06 BA D0 BF 02 17"),
            new Option("r214 >> r217", "4C C7 99 C4 6D F4 95 46 5F E5 77 45 E6 A1 37 3F 02 17"),
            new Option("r214 >> r218", "26 2B F1 C6 7A 96 03 C5 39 08 30 C0 DE AE BE 3F 02 18"),
            new Option("r215 >> r218", "92 20 BC 46 A0 E8 3A 45 5C 82 04 43 36 94 F0 BF 02 18"),
            new Option("r201 >> r219", "00 04 0A 42 00 00 D1 3B CD B4 9B 44 38 6E 44 40 02 19"),
            new Option("r211 >> r219", "B8 29 9F C7 00 00 36 BB 23 9D AF C8 A0 F8 3B BD 02 19"),
            new Option("r219 >> r219", "1E DD 9A C7 00 00 AC BB 28 A2 AB C8 3C 7F 40 C0 02 19"),
            new Option("r219 >> r219", "9E 45 FF 42 00 00 AC BB 43 06 D4 C5 F8 AD 1E 3E 02 19"),
            new Option("r21b >> r21a", "B1 9B B7 C6 47 EE AA C5 F9 6C 2E C7 20 54 E9 BF 02 1A"),
            new Option("r225 >> r21a", "ED C8 C3 C6 69 D2 9A 45 19 B8 90 47 35 8A 46 40 02 1A"),
            new Option("r21a >> r21b", "3A 58 62 C8 AC CE 85 C5 DB DA 2B C7 72 DC AE 3F 02 1B"),
            new Option("r225 >> r21b", "8A B8 49 C8 C6 16 49 C7 F0 09 0D 48 98 AB 46 40 02 1B"),
            new Option("r224 >> r21d", "F4 28 E0 C6 4C 2C 70 C5 C4 39 1B C7 66 DE A9 3F 02 1D"),
            new Option("r225 >> r21d", "5D 2A 4C 46 BF 1A 68 46 9A D8 B3 47 57 00 C7 BF 02 1D"),
            new Option("r221 >> r220", "0C 9F C5 44 B2 42 91 44 86 9F F1 45 21 83 04 C0 02 20"),
            new Option("r223 >> r220", "99 B2 99 44 00 00 72 BB 1B 14 B9 C5 33 B3 22 3F 02 20"),
            new Option("r22b >> r220", "41 9A CF 44 6D 42 91 44 DA 63 F5 45 E3 51 0A C0 02 20"),
            new Option("r22c >> r220", "10 27 8B 45 00 00 83 BB F2 65 28 C5 B2 7B 14 C0 02 20"),
            new Option("r220 >> r221", "18 66 9E 45 F6 23 51 44 8C 66 5D 45 80 75 0C C0 02 21"),
            new Option("r229 >> r221", "F3 BD 2A C7 7E 24 51 44 4E 39 6A 45 F0 DB C9 3F 02 21"),
            new Option("r210 >> r222", "C9 78 A7 44 E0 31 56 42 8B B7 87 C5 1C 5A C4 BF 02 22"),
            new Option("r220 >> r223", "8B BE E5 C6 F1 EF 80 45 D5 A3 DD 46 84 0F FF BF 02 23"),
            new Option("r224 >> r223", "1B 5B 0B 46 D4 41 BC 42 57 A5 DA C6 CC 4F FE BF 02 23"),
            new Option("r21d >> r224", "C9 93 AD 46 00 00 98 BB 7F 65 0E C6 10 B1 B8 BF 02 24"),
            new Option("r223 >> r224", "48 54 A0 C6 00 00 0C BB 70 08 05 C5 3E 9F 9E 3F 02 24"),
            new Option("r21a >> r225", "E8 77 9B 47 E5 7F BB 44 32 0C 62 C6 00 0F EF BE 02 25"),
            new Option("r21b >> r225", "EF ED 87 47 D7 7F BB 44 6A E5 FA C5 2B 68 38 C0 02 25"),
            new Option("r21d >> r225", "4B 49 F9 C6 7A 45 F3 44 0E 46 4C C3 E4 79 35 C0 02 25"),
            new Option("r226 >> r225", "47 96 78 20 44 BB 80 34 C6 AE 1F C1 BF 8E E5 21 02 25"),
            new Option("r225 >> r226", "2E 2B 5B C5 FE 3F 9C 45 B5 41 A9 46 07 0C 44 40 02 26"),
            new Option("r226 >> r227", "F0 1B 36 46 E8 05 7A 42 B1 65 CD C2 EA 54 DC BF 02 27"),
            new Option("r228 >> r227", "64 27 53 43 FB 4B 1A 47 03 43 8F 46 88 A6 60 3D 02 27"),
            new Option("r227 >> r228", "57 9E 75 C1 FE E7 80 45 35 D1 74 46 EE D0 40 40 02 28"),
            new Option("r22a >> r228", "98 1C 2A 42 74 8D 7C 45 B7 3B 5F C6 C4 F6 23 BE 02 28"),
            new Option("r215 >> r229", "27 23 0F C5 5A 64 77 C6 17 06 6F 46 9B 09 EA 3F 02 29"),
            new Option("r221 >> r229", "79 55 96 46 8A 3A 01 C6 13 95 71 C7 43 E7 CF BF 02 29"),
            new Option("r228 >> r22a", "CE 07 4E C2 00 2C 49 47 63 AD 8E C6 82 D5 40 40 02 2A"),
            new Option("r22a >> r22a", "52 8E 70 46 04 D0 04 C6 11 57 18 C7 90 9E C7 3F 02 2A"),
            new Option("r22a >> r22a", "0C A1 67 46 00 08 CF 46 21 65 18 C7 1C 7E CA BF 02 2A"),
            new Option("r204 >> r22c", "A6 62 E5 C3 00 00 8E BB BE 5F 77 43 C4 8A 47 40 02 2C"),
            new Option("r211 >> r22c", "0B 6E 25 C2 00 00 36 BB A5 9A 68 C2 CC 6B 35 40 02 2C"),
            new Option("r220 >> r22c", "6A E4 1C C3 00 00 F0 BA E4 66 FA 41 00 00 40 40 02 2C"),
            new Option("r305 >> r22c", "5C EE 83 C1 00 00 C8 BA B6 7C AD 43 D6 71 40 40 02 2C"),
            new Option("r31d >> r22c", "8F 97 35 C2 00 00 AC BB DD 0C 44 43 78 2F 45 40 02 2C"),
            new Option("r22a >> r300", "09 66 E2 46 02 D0 84 C6 7A CD 20 C7 70 C3 1B C0 03 00"),
            new Option("r301 >> r300", "BA 6E BE C5 05 20 4B C6 CE 12 08 46 5A 9B B3 3F 03 00"),
            new Option("r300 >> r301", "3D 92 02 C6 08 C0 DA C5 23 43 FF 45 6F A4 1A C0 03 01"),
            new Option("r303 >> r301", "50 99 C0 46 F0 49 6A C5 86 AC 39 46 86 21 08 C0 03 01"),
            new Option("r301 >> r303", "0D A6 B9 C6 00 00 B2 3B 20 4A AB C6 24 FA B0 3F 03 03"),
            new Option("r304 >> r303", "DE 16 30 C6 00 00 72 BB CC 27 2A 46 AA C3 48 C0 03 03"),
            new Option("r306 >> r303", "59 5C 8A C6 00 00 4A BB 29 53 59 C6 6A 1D DD BF 03 03"),
            new Option("r303 >> r304", "72 10 8A C6 00 00 36 BB 9F F5 BD 44 29 B3 BB BD 03 04"),
            new Option("r305 >> r304", "05 72 94 C4 00 00 8E BB C7 A6 76 C5 74 D3 CE 3E 03 04"),
            new Option("r22c >> r305", "EE 4D FF 44 00 00 0C BB 92 67 A1 C3 E0 E6 1A BD 03 05"),
            new Option("r304 >> r305", "11 C6 F9 45 00 00 9C BA CB 00 02 46 0C B2 2A C0 03 05"),
            new Option("r306 >> r305", "85 8E 5D C5 00 00 6E BB A6 C1 01 C6 42 8A 0F BF 03 05"),
            new Option("r303 >> r306", "0E EB 86 C5 00 62 16 3F 9A D4 0D 45 D7 7F B8 3F 03 06"),
            new Option("r305 >> r306", "26 61 50 45 00 14 16 3F BF 64 2A 45 7C D7 26 40 03 06"),
            new Option("r307 >> r306", "85 3F 83 45 39 09 7A 45 97 3C B4 46 E8 FF C4 3F 03 06"),
            new Option("r308 >> r306", "E2 77 16 C6 66 09 7A 45 74 60 A5 45 28 AA D0 BD 03 06"),
            new Option("r30b >> r306", "D4 F1 5B C6 B2 44 9C 45 1D D1 D5 C5 28 CC 24 BE 03 06"),
            new Option("r306 >> r307", "65 09 8A 44 00 00 8E BB 38 EC 7A 45 68 6D F0 BF 03 07"),
            new Option("r306 >> r308", "BE FF EE C4 00 00 72 BB 64 17 6C 45 4E 91 3D 40 03 08"),
            new Option("r30a >> r309", "D6 53 F6 45 00 1F 00 40 DF C7 61 46 7E 0A FA BF 03 09"),
            new Option("r30e >> r309", "70 BC 04 C6 00 00 83 BB EE 88 E6 C5 CC 21 BF 3F 03 09"),
            new Option("r309 >> r30a", "38 E6 15 C3 E7 28 78 C5 98 4D D8 46 A4 B9 AE 3F 03 0A"),
            new Option("r306 >> r30b", "82 47 91 C6 8C E2 79 44 43 78 44 45 B2 40 39 40 03 0B"),
            new Option("r30c >> r30b", "6A 94 8F 46 00 00 A9 3B C2 52 D6 45 E0 6C EC BE 03 0B"),
            new Option("r30b >> r30c", "E9 EF 24 46 00 00 20 BB CE B4 E1 45 84 7B 37 40 03 0C"),
            new Option("r30e >> r30c", "2B 77 6B C5 00 00 BC 3B 26 AD F2 C5 02 FF 87 BE 03 0C"),
            new Option("r30f >> r30d", "DF 01 14 46 00 00 36 BB 29 1E C1 46 48 CD CA BF 03 0D"),
            new Option("r311 >> r30d", "6F 6D 8E C5 00 00 9C BA E2 64 90 C6 48 17 92 BD 03 0D"),
            new Option("r334 >> r30d", "DF 01 14 46 00 00 36 BB 29 1E C1 46 48 CD CA BF 03 0D"),
            new Option("r309 >> r30e", "7D 83 7B C4 A8 D6 07 C5 D6 33 60 46 E4 30 D4 BF 03 0E"),
            new Option("r30c >> r30e", "7F E1 51 C6 94 D3 D9 44 B8 10 B0 C6 12 C0 3B 40 03 0E"),
            new Option("r30d >> r30f", "01 EC 5E C7 00 D8 06 C7 86 32 D5 47 40 4C BC 3F 03 0F"),
            new Option("r312 >> r30f", "01 EC 5E C7 00 D8 06 C7 86 32 D5 47 40 4C BC 3F 03 0F"),
            new Option("r311 >> r310", "81 03 22 C6 00 00 BE 3B 30 D6 4D C6 8E 32 D8 BF 03 10"),
            new Option("r30d >> r311", "FF CC FE 43 00 00 72 BB 55 DE 66 45 E4 02 42 40 03 11"),
            new Option("r310 >> r311", "9E 51 E7 C5 00 00 36 BB 80 83 85 C6 40 50 C8 3F 03 11"),
            new Option("r312 >> r311", "FF CC FE 43 00 00 72 BB 55 DE 66 45 E4 02 42 40 03 11"),
            new Option("r30d >> r312", "15 31 AB C5 00 00 0C BB 63 D7 CF 46 9E DA D3 BF 03 12"),
            new Option("r30f >> r315", "5E 56 FA 45 00 00 4A BB 2E D0 8D C5 69 2F E1 BE 03 15"),
            new Option("r316 >> r315", "62 C5 3D C6 00 00 BC 3B BD 58 A4 C5 E7 F5 40 40 03 15"),
            new Option("r334 >> r315", "5E 56 FA 45 00 00 4A BB 2E D0 8D C5 69 2F E1 BE 03 15"),
            new Option("r315 >> r316", "4B 4D 00 C3 00 00 9C BA 20 B7 F3 C5 80 60 10 BE 03 16"),
            new Option("r317 >> r316", "2F 5B 15 47 F4 5F 9F C5 4F 7F B6 46 78 B6 47 C0 03 16"),
            new Option("r316 >> r317", "62 45 00 C6 04 60 9F C5 D0 F7 26 43 70 21 90 BE 03 17"),
            new Option("r318 >> r317", "63 11 EC 46 00 00 C6 3B 3F 30 08 C7 DC 31 DD BF 03 17"),
            new Option("r317 >> r318", "54 20 C6 C6 00 AC 49 BF CB CE 6D 45 18 C5 B6 3F 03 18"),
            new Option("r31a >> r318", "46 D4 DA 40 44 4E 82 C4 45 81 8B 2D BF CE 86 BE 03 18"),
            new Option("r318 >> r31a", "45 9D 7E 3E 3B BC 00 00 C1 7F 61 51 3F 9E 98 08 03 1A"),
            new Option("r31b >> r31a", "58 5E 46 47 07 38 47 C5 27 44 27 42 1A 47 EB BF 03 1A"),
            new Option("r31a >> r31b", "83 07 A0 C7 25 C0 5A 45 E3 A6 87 C6 34 B2 8A 3F 03 1B"),
            new Option("r31d >> r31b", "E9 46 B1 47 0A DD EA 45 4D 8A B8 C5 C0 EE 36 C0 03 1B"),
            new Option("r31d >> r31c", "81 DD 22 47 11 00 7A 45 82 DF 17 47 9F 61 45 C0 03 1C"),
            new Option("r320 >> r31c", "F3 E8 5E C6 00 00 80 BB CC EF 99 C6 70 3F B2 BD 03 1C"),
            new Option("r22c >> r31d", "6E E4 FA 46 64 DE 49 C6 D0 79 33 C7 32 99 4C 3F 03 1D"),
            new Option("r31b >> r31d", "17 36 EE C5 21 FC 3B C5 41 65 6F C5 3A 10 94 3F 03 1D"),
            new Option("r31c >> r31d", "15 AA 13 47 48 98 A1 C5 88 45 63 C7 40 64 FA BC 03 1D"),
            new Option("r31c >> r320", "CA 14 EB 46 E6 01 BD 45 A8 42 86 47 24 2D 34 C0 03 20"),
            new Option("r321 >> r320", "AB FD D3 46 A0 3D CC 45 8C 41 4D C6 B0 A0 81 3F 03 20"),
            new Option("r320 >> r321", "82 67 C3 46 16 1D CF 45 0A 7E 68 C6 6D 5B 35 C0 03 21"),
            new Option("r325 >> r321", "42 4F BD 46 AC DE 55 46 7C 8E 8F C7 3E 92 79 3F 03 21"),
            new Option("r321 >> r325", "F7 AB 60 46 6C 2A EC 45 90 BB C1 C6 ED 1C 3F BF 03 25"),
            new Option("r326 >> r325", "AB 0A 26 45 9E 4F 1F 45 83 B6 FF C5 C9 70 D8 3E 03 25"),
            new Option("r325 >> r326", "54 FE F2 43 15 77 6E 45 A2 EE 02 C6 94 08 C3 BF 03 26"),
            new Option("r327 >> r326", "52 60 D1 C5 0D D9 A3 45 24 38 B7 46 68 C3 3C 40 03 26"),
            new Option("r326 >> r327", "CB 34 CF C5 E1 57 A4 45 3C 48 C6 46 44 65 DC BD 03 27"),
            new Option("r329 >> r327", "D2 46 76 C6 20 41 40 45 4D F7 62 45 2C 44 BC 3F 03 27"),
            new Option("r327 >> r329", "46 CA 3A 46 BA 38 6F C4 3A 13 BC C5 EA 07 D1 BF 03 29"),
            new Option("r330 >> r329", "98 94 1B 46 52 BD 76 C5 69 4F 54 46 FB 70 48 40 03 29"),
            new Option("r330 >> r329", "FE 44 A9 45 1C C9 66 C4 FD 8E 70 46 28 51 F6 BF 03 29"),
            new Option("r329 >> r330", "62 E9 A5 C4 19 FC 90 C5 03 A1 1E C6 5C CD D5 BD 03 30"),
            new Option("r329 >> r330", "32 0D 75 C5 8B A4 CD C4 3F 7F 1D C6 4D B9 BA 3F 03 30"),
            new Option("r331 >> r330", "A7 27 39 C6 9C EA F1 44 71 5C 93 C5 B0 C0 14 40 03 30"),
            new Option("r330 >> r331", "1A 61 F5 C6 84 A6 CD 45 01 5C FC 46 F4 BE F5 BE 03 31"),
            new Option("r332 >> r331", "74 2B 25 C7 39 B6 F1 44 2D BB 37 47 1D EA E9 3F 03 31"),
            new Option("r331 >> r332", "0B 1B 25 C7 77 0D 77 46 FD B8 36 47 8C 06 AD 3F 03 32"),
            new Option("r331 >> r333", "99 4A 28 C9 36 E2 AB C6 6D F9 13 C9 00 6C AA BF 03 33"),
            new Option("r30f >> r334", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 03 34"),
            new Option("r406 >> r405", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 05"),
            new Option("r405 >> r406", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 06"),
            new Option("r40a >> r406", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 06"),
            new Option("r406 >> r40a", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 0A"),
            new Option("r40b >> r40a", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 0A"),
            new Option("r40c >> r40a", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 0A"),
            new Option("r40f >> r40a", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 0A"),
            new Option("r40a >> r40b", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 0B"),
            new Option("r40a >> r40c", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 0C"),
            new Option("r40a >> r40f", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 0F"),
            new Option("r410 >> r40f", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 0F"),
            new Option("r40f >> r410", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 10"),
            new Option("r411 >> r410", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 10"),
            new Option("r40d >> r411", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 11"),
            new Option("r410 >> r411", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 11"),
            new Option("r40e >> r40d", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 0D"),
            new Option("r411 >> r40d", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 0D"),
            new Option("r40d >> r40e", "92 F8 B1 46 9C 4A 1E C7 57 27 C6 C6 2B 09 C8 3F 04 0E"),
            new Option("r500 >> r501", "D8 D6 99 C6 00 00 83 BB 9F 1F 95 C6 99 E8 7C 3F 05 01"),
            new Option("r502 >> r501", "D7 8F 9B C5 00 80 04 C6 CE 35 4D 47 1E 52 1E C0 05 01"),
            new Option("r501 >> r502", "9D 80 2B C7 58 9A 16 C2 73 45 66 C6 D7 A3 00 40 05 02"),
            new Option("r502 >> r503", "00 00 0C 43 00 00 82 C4 00 E0 C9 45 DB 0F 49 40 05 03"),
            new Option("r505 >> r504", "AF CB 19 47 93 17 48 45 11 55 D0 C6 CF 7C C0 BF 05 04"),
            new Option("r506 >> r504", "14 7B 5C C6 B0 8E 29 45 C0 6A 03 47 74 1A 33 40 05 04"),
            new Option("r504 >> r505", "0F F9 39 C7 60 46 3D 41 7D 5C DD 45 8E CA F1 3F 05 05"),
            new Option("r507 >> r505", "10 F6 13 46 68 97 17 42 81 B7 3D C4 8B 3F 13 C0 05 05"),
            new Option("r503 >> r506", "CB 95 58 46 C6 3D C6 45 51 74 73 45 24 8E BD BF 05 06"),
            new Option("r504 >> r506", "35 25 8E 47 AF DA 0B C5 75 59 12 C7 3A F1 9C BF 05 06"),
            new Option("r505 >> r507", "05 14 2E 46 06 42 5F C4 42 17 F0 C2 9B DB 56 3F 05 07"),
            new Option("r506 >> r508", "00 70 7F 45 00 C8 82 C6 00 19 D5 C7 6C 1D 05 40 05 08"),
            new Option("r50a >> r508", "CA 6C B1 C6 00 00 B2 3B B3 57 12 47 B6 98 EE 3F 05 08"),
            new Option("r50b >> r508", "D0 87 5B 47 3E 0C F4 C6 A9 DB 2C C8 02 CE B8 BF 05 08"),
            new Option("r50a >> r509", "44 EE CC 47 6A 80 A7 C4 1B 9F 9C C6 86 A7 39 40 05 09"),
            new Option("r508 >> r50a", "F6 1D 77 47 E0 13 CD C1 A0 EB DF 46 9B C2 90 BF 05 0A"),
            new Option("r509 >> r50a", "47 41 B7 43 D0 25 CD C1 6B 43 7A C3 0C 01 1A BE 05 0A"),
            new Option("r508 >> r50b", "00 40 F1 45 00 00 7F C4 00 F0 25 C6 DE 7E BC 3F 05 0B"),
            new Option("r510 >> r50c", "D6 43 27 C6 08 00 FA 45 27 83 81 C5 80 61 C0 3F 05 0C"),
            new Option("r50e >> r50d", "16 8E 13 C7 21 2A F7 44 77 FE A1 C7 00 D9 CE BC 05 0D"),
            new Option("r50f >> r50d", "20 B9 3F 46 80 BB 41 C0 C2 9D 06 C7 38 9E 24 BE 05 0D"),
            new Option("r509 >> r50e", "62 86 0B 47 00 00 40 BA D2 76 11 C7 A5 CD 8D 3F 05 0E"),
            new Option("r50d >> r50e", "7C F7 D2 C3 9B EF BC 45 E2 A2 70 44 80 7A 41 40 05 0E"),
            new Option("r50f >> r50e", "B9 EE EA 46 F8 FF 79 45 4D 0F 42 46 34 5A 45 40 05 0E"),
            new Option("r50d >> r50f", "0A AA 9F 46 FA BF 5A 45 01 04 0D 47 44 F0 44 C0 05 0F"),
            new Option("r50e >> r50f", "9F B3 47 C2 F5 BF 5A 45 AD EB 13 C4 24 A1 96 3E 05 0F"),
            new Option("r510 >> r50f", "92 B4 19 47 F4 BF 5A 45 21 E4 26 46 14 47 9E BF 05 0F"),
            new Option("r50c >> r510", "19 47 AB 47 F8 DF AB 45 D5 9D 2B 47 86 A5 CC BF 05 10"),
            new Option("r50f >> r510", "9A 3B 21 47 F7 BF 5A 45 0D E3 CC 46 28 F2 BB 3F 05 10"),
            new Option("r50c >> r512", "B5 57 59 C7 5B 44 1E C7 E9 94 41 C5 D2 47 3C 40 05 12"),
            new Option("r513 >> r512", "5F CC 5C 46 E0 4B 08 C7 F3 09 FB C6 70 DC FF BD 05 12"),
            new Option("r512 >> r513", "56 1D 80 C7 FC FF F9 C4 B5 A9 4D C6 58 39 07 40 05 13"),
            new Option("r514 >> r513", "14 BD 0C 47 00 00 44 BB 32 FE 1E C6 BE 9E F5 BF 05 13"),
            new Option("r513 >> r514", "38 33 1B 47 DE 13 8B 43 A5 7B 19 C7 B8 54 D8 3F 05 14"),
            new Option("r516 >> r515", "62 C5 3D C6 00 00 BC 3B BD 58 A4 C5 E7 F5 40 40 05 15"),
            new Option("r514 >> r516", "CF F0 0B C5 70 75 0B 42 3E 2A 6F C5 E7 E1 C7 3E 05 16"),
            new Option("r515 >> r516", "4B 4D 00 C3 00 00 9C BA 20 B7 F3 C5 80 60 10 BE 05 16"),
            new Option("r517 >> r516", "2F 5B 15 47 F4 5F 9F C5 4F 7F B6 46 78 B6 47 C0 05 16"),
            new Option("r516 >> r517", "62 45 00 C6 04 60 9F C5 D0 F7 26 43 70 21 90 BE 05 17"),
            new Option("r517 >> r518", "BE 5C DF 46 DC 01 BD 45 30 3F 84 47 D9 CC 3E C0 05 18"),
            new Option("r519 >> r518", "AB FD D3 46 A0 3D CC 45 8C 41 4D C6 B0 A0 81 3F 05 18"),
            new Option("r518 >> r519", "82 67 C3 46 16 1D CF 45 0A 7E 68 C6 6D 5B 35 C0 05 19"),
            new Option("r51a >> r519", "42 4F BD 46 AC DE 55 46 7C 8E 8F C7 3E 92 79 3F 05 19"),
            new Option("r519 >> r51a", "F7 AB 60 46 6C 2A EC 45 90 BB C1 C6 ED 1C 3F BF 05 1A"),
            new Option("r51a >> r51b", "54 FE F2 43 15 77 6E 45 A2 EE 02 C6 94 08 C3 BF 05 1B"),
            new Option("r51c >> r51b", "52 60 D1 C5 0D D9 A3 45 24 38 B7 46 68 C3 3C 40 05 1B"),
            new Option("r51b >> r51c", "7C 72 C0 C5 DF 57 A4 45 29 FD C4 46 16 2B 95 3F 05 1C"),
            new Option("r51c >> r51d", "E6 1A 2C 46 66 9F 01 45 41 B7 34 C6 C6 32 68 BF 05 1D"),
            new Option("r51d >> r51e", "00 20 91 C6 00 1E B6 46 00 20 21 47 00 00 6E BF 05 1E"), // Custom
        };

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        const uint PROCESS_VM_WRITE = 0x0020;
        const uint PROCESS_VM_OPERATION = 0x0008;
        const uint PROCESS_VM_READ = 0x0010;
        const uint PROCESS_QUERY_INFORMATION = 0x0400;

        public MainForm()
        {
            Text = "Teleport Tool by tomazm";
            Width = 530;
            Height = 630;
            StartPosition = FormStartPosition.CenterScreen;

            listBox = new ListBox
            {
                Dock = DockStyle.Top,
                Height = 420,
                Font = new System.Drawing.Font("Consolas", 9)
            };
            foreach (var o in Options)
                listBox.Items.Add(o.Label + "    " + BitConverter.ToString(o.Values).Replace("-", " "));
            Controls.Add(listBox);

            var panel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 170
            };
            Controls.Add(panel);

            var lblAddr = new Label
            {
                Text = "Target address (hex):",
                Left = 10,
                Top = 10,
                Width = 140
            };
            panel.Controls.Add(lblAddr);

            txtAddress = new TextBox
            {
                Text = "0x00000000",
                Left = 150,
                Top = 8,
                Width = 120
            };
            panel.Controls.Add(txtAddress);

            btnTeleport = new Button
            {
                Text = "Teleport",
                Left = 280,
                Top = 6,
                Width = 80
            };
            btnTeleport.Click += BtnTeleport_Click;
            panel.Controls.Add(btnTeleport);

            btnScan = new Button
            {
                Text = "Find Address",
                Left = 365,
                Top = 6,
                Width = 80
            };
            btnScan.Click += BtnScan_Click;
            panel.Controls.Add(btnScan);

            lblStatus = new Label
            {
                Text = "Select an option and click Teleport. Run as admin for memory access.",
                Left = 10,
                Top = 40,
                Width = 430
            };
            panel.Controls.Add(lblStatus);

            lblCount = new Label
            {
                Text = "Options loaded: " + Options.Count,
                Left = 10,
                Top = 70,
                Width = 430
            };
            panel.Controls.Add(lblCount);
        }

        private void BtnTeleport_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex < 0)
            {
                lblStatus.Text = "No option selected.";
                return;
            }
            Option opt = Options[listBox.SelectedIndex];
            IntPtr addr;
            try
            {
                addr = new IntPtr(Convert.ToInt64(txtAddress.Text, 16));
            }
            catch
            {
                lblStatus.Text = "Invalid address.";
                return;
            }

            Process[] procs = Process.GetProcessesByName("game");
            if (procs.Length == 0)
            {
                lblStatus.Text = "game.exe not running.";
                return;
            }
            int pid = procs[0].Id;
            IntPtr hProc = OpenProcess(PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_QUERY_INFORMATION, false, pid);
            if (hProc == IntPtr.Zero)
            {
                lblStatus.Text = "OpenProcess failed. Run as admin.";
                return;
            }
            int written = 0;
            bool ok = WriteProcessMemory(hProc, addr, opt.Values, opt.Values.Length, out written);
            CloseHandle(hProc);
            if (ok)
                lblStatus.Text = "Teleported: " + opt.Label + " (" + written + " bytes)";
            else
                lblStatus.Text = "WriteProcessMemory failed.";
        }

        private void BtnScan_Click(object sender, EventArgs e)
        {
            Process[] procs = Process.GetProcessesByName("game");
            if (procs.Length == 0)
            {
                lblStatus.Text = "game.exe not running.";
                return;
            }
            int pid = procs[0].Id;
            IntPtr hProc = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, pid);
            if (hProc == IntPtr.Zero)
            {
                lblStatus.Text = "OpenProcess (read) failed. Run as admin.";
                return;
            }
            // search for any option's byte pattern
            IntPtr foundAddr = IntPtr.Zero;
            foreach (var opt in Options)
            {
                foundAddr = SearchMemory(hProc, opt.Values);
                if (foundAddr != IntPtr.Zero) break;
            }
            CloseHandle(hProc);
            if (foundAddr != IntPtr.Zero)
            {
                txtAddress.Text = "0x" + foundAddr.ToInt64().ToString("X");
                lblStatus.Text = "Address found: " + txtAddress.Text;
            }
            else
            {
                lblStatus.Text = "Pattern not found in memory. Teleport once first, then scan.";
            }
        }

        private IntPtr SearchMemory(IntPtr hProc, byte[] needle)
        {
            IntPtr addr = IntPtr.Zero;
            long current = 0;
            MEMORY_BASIC_INFORMATION mbi;
            int mbiSize = Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));
            while (true)
            {
                int r = VirtualQueryEx(hProc, new IntPtr(current), out mbi, mbiSize);
                if (r == 0) break;
                long baseAddr = mbi.BaseAddress.ToInt64();
                long regionSize = mbi.RegionSize.ToInt64();
                if (mbi.State == 0x1000 && (mbi.Protect == 0x02 || mbi.Protect == 0x04 || mbi.Protect == 0x20 || mbi.Protect == 0x40 || mbi.Protect == 0x80))
                {
                    long offset = 0;
                    while (offset < regionSize)
                    {
                        int chunk = (int)Math.Min(1024 * 1024, regionSize - offset);
                        byte[] buf = new byte[chunk];
                        int read = 0;
                        if (ReadProcessMemory(hProc, new IntPtr(baseAddr + offset), buf, chunk, out read) && read > 0)
                        {
                            int idx = IndexOf(buf, needle, read);
                            if (idx >= 0)
                                return new IntPtr(baseAddr + offset + idx);
                        }
                        offset += chunk;
                    }
                }
                long next = baseAddr + regionSize;
                if (next <= current) break;
                current = next;
                if (current < 0) break;
            }
            return IntPtr.Zero;
        }

        private int IndexOf(byte[] haystack, byte[] needle, int haystackLen)
        {
            if (needle.Length == 0) return 0;
            for (int i = 0; i <= haystackLen - needle.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < needle.Length; j++)
                {
                    if (haystack[i + j] != needle[j]) { match = false; break; }
                }
                if (match) return i;
            }
            return -1;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
