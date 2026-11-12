using System.Buffers.Binary;

namespace MiloLib.Utils.Conversion;

internal static class WiiTextureCodec
{
    // todo: this whole thing just feels pretty crappy and hard to even comprehend, it could do with a rewrite

    public static byte[] DecodeCmp(byte[] data, int width, int height)
    {
        byte[] output = new byte[width * height * 4];
        int blocksX = width / 8;
        int blocksY = height / 8;
        int srcOffset = 0;

        for (int by = 0; by < blocksY; by++)
        {
            for (int bx = 0; bx < blocksX; bx++)
            {
                DecodeDxt1SubBlock(data, ref srcOffset, output, width, bx * 8 + 0, by * 8 + 0);
                DecodeDxt1SubBlock(data, ref srcOffset, output, width, bx * 8 + 4, by * 8 + 0);
                DecodeDxt1SubBlock(data, ref srcOffset, output, width, bx * 8 + 0, by * 8 + 4);
                DecodeDxt1SubBlock(data, ref srcOffset, output, width, bx * 8 + 4, by * 8 + 4);
            }
        }

        return output;
    }

    private static void DecodeDxt1SubBlock(byte[] data, ref int offset, byte[] output, int width, int baseX, int baseY)
    {
        ushort color0 = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset, 2));
        offset += 2;
        ushort color1 = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset, 2));
        offset += 2;
        uint lookupTable = BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(offset, 4));
        offset += 4;

        byte[] c0 = DecodeRgb565(color0);
        byte[] c1 = DecodeRgb565(color1);

        byte[] c2 = new byte[4];
        byte[] c3 = new byte[4];
        if (color0 > color1)
        {
            for (int i = 0; i < 3; i++)
            {
                c2[i] = (byte)((2 * c0[i] + c1[i]) / 3);
                c3[i] = (byte)((c0[i] + 2 * c1[i]) / 3);
            }
            c2[3] = 255;
            c3[3] = 255;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                c2[i] = (byte)((c0[i] + c1[i]) / 2);
            }

            c2[3] = 255;
        }

        for (int py = 0; py < 4; py++)
        {
            for (int px = 0; px < 4; px++)
            {
                int pixelIndex = py * 4 + px;
                int colorIndex = (int)((lookupTable >> (30 - pixelIndex * 2)) & 0x03);
                byte[] color = colorIndex switch
                {
                    0 => c0,
                    1 => c1,
                    2 => c2,
                    _ => c3
                };

                int outOffset = ((baseY + py) * width + baseX + px) * 4;
                output[outOffset + 0] = color[0];
                output[outOffset + 1] = color[1];
                output[outOffset + 2] = color[2];
                output[outOffset + 3] = color[3];
            }
        }
    }

    private static byte[] DecodeRgb565(ushort color)
    {
        int r5 = (color >> 11) & 0x1F;
        int g6 = (color >> 5) & 0x3F;
        int b5 = color & 0x1F;

        return
        [
            (byte)((r5 << 3) | (r5 >> 2)),
            (byte)((g6 << 2) | (g6 >> 4)),
            (byte)((b5 << 3) | (b5 >> 2)),
            255
        ];
    }
}
