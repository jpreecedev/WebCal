package smartcardreader;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import javax.smartcardio.CardChannel;
import javax.smartcardio.CardException;
import javax.smartcardio.CommandAPDU;
import javax.smartcardio.ResponseAPDU;

public class Utilities {

    public static byte[] getCalibrationData(CardChannel channel) throws CardException, IOException {
        byte[] arrayOfByte = null;
        if ((selectApplication(channel)) && (selectFile(1290, channel))) {
            int i = toInt(readBlock(2, 1, channel));

            arrayOfByte = readBlock(3 + i * 105, 105, channel);
        }
        return arrayOfByte;
    }

    public static boolean isWorkshopCard(CardChannel channel) throws CardException, IOException {
        selectFile(1281, channel);
        byte[] arrayOfByte = readBlock(0, 1, channel);
        if (arrayOfByte != null) {
            return arrayOfByte[0] == 2;
        }
        return false;
    }

    public static boolean selectApplication(CardChannel channel) throws CardException {
        byte[] arrayOfByte = {-1, 84, 65, 67, 72, 79};
        CommandAPDU localCommandAPDU = new CommandAPDU(0, -92, 4, 12, arrayOfByte);

        ResponseAPDU localResponseAPDU = channel.transmit(localCommandAPDU);
        return localResponseAPDU.getSW() == 36864;
    }

    public static boolean selectFile(int paramInt, CardChannel channel) throws CardException {
        byte[] arrayOfByte = asWord(paramInt);

        CommandAPDU localCommandAPDU = new CommandAPDU(0, -92, 2, 12, arrayOfByte);
        ResponseAPDU localResponseAPDU = channel.transmit(localCommandAPDU);

        return localResponseAPDU.getSW() == 36864;
    }

    public static byte hiByte(int paramInt) {
        return (byte) (paramInt >> 8 & 0xFF);
    }

    public static byte lowByte(int paramInt) {
        return (byte) (paramInt & 0xFF);
    }

    public static byte[] asWord(int paramInt) {
        byte[] arrayOfByte = {0, 0};
        arrayOfByte[0] = hiByte(paramInt);
        arrayOfByte[1] = lowByte(paramInt);

        return arrayOfByte;
    }

    public static int toInt(byte[] paramArrayOfByte) {
        int i = 0;
        for (int j = 0; j < paramArrayOfByte.length; j++) {
            int k = (paramArrayOfByte.length - 1 - j) * 8;
            i += ((paramArrayOfByte[j] & 0xFF) << k);
        }
        return i;
    }

    public static boolean selectByName2(byte[] paramArrayOfByte, CardChannel channel) throws CardException {
        CommandAPDU localCommandAPDU = new CommandAPDU(0, -92, 4, 12, paramArrayOfByte);
        ResponseAPDU localResponseAPDU = channel.transmit(localCommandAPDU);

        return localResponseAPDU.getSW() == 36864;
    }

    private static String toString(byte[] paramArrayOfByte) {
        StringBuilder localStringBuilder = new StringBuilder();
        char[] arrayOfChar = new char[2];

        for (int i = 0; i < paramArrayOfByte.length; i++) {
            int j = (paramArrayOfByte[i] & 0xF0) >>> 4;
            arrayOfChar[0] = "0123456789ABCDEF".charAt(j);
            j = paramArrayOfByte[i] & 0xF;
            arrayOfChar[1] = "0123456789ABCDEF".charAt(j);
            localStringBuilder.append(arrayOfChar);
            localStringBuilder.append(' ');
        }

        return localStringBuilder.toString();
    }

    public static long toLong(byte[] paramArrayOfByte) {
        long l = 0L;
        for (int i = 0; i < paramArrayOfByte.length; i++) {
            int j = (paramArrayOfByte.length - 1 - i) * 8;
            l += ((paramArrayOfByte[i] & 0xFF) << j);
        }
        return l;
    }
    
      public static String parseBCD(byte[] paramArrayOfByte)
  {
    StringBuilder localStringBuilder = new StringBuilder();

    for (int k : paramArrayOfByte) {
      char c1 = parseBCDByte((byte)(k >> 4 & 0xF));
      char c2 = parseBCDByte((byte)(k & 0xF));

      localStringBuilder.append(c1);
      localStringBuilder.append(c2);
    }

    return localStringBuilder.toString();
  }
      
        private static char parseBCDByte(byte paramByte)
  {
    return (char)(48 + paramByte);
  }

    public static byte[] readBlock(int paramInt1, int paramInt2, CardChannel channel) throws CardException, IOException {
        ByteArrayOutputStream localByteArrayOutputStream = new ByteArrayOutputStream(paramInt2);

        int i = paramInt1;

        while (i < paramInt1 + paramInt2) {
            int j = Math.min(255, paramInt1 + paramInt2 - i);

            CommandAPDU localCommandAPDU = new CommandAPDU(0, -80, hiByte(i), lowByte(i), j);

            ResponseAPDU localResponseAPDU = channel.transmit(localCommandAPDU);
            try {
                localByteArrayOutputStream.write(localResponseAPDU.getData());
            } catch (IOException localIOException) {
                System.out.println(new StringBuilder().append("ERROR: ").append(localIOException).toString());
                return null;
            }

            i += j;
        }

        return localByteArrayOutputStream.toByteArray();
    }
}
