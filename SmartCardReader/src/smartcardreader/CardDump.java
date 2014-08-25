package smartcardreader;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.smartcardio.CardChannel;
import javax.smartcardio.CardException;
import javax.smartcardio.CommandAPDU;
import javax.smartcardio.ResponseAPDU;

public class CardDump {

    private WorkshopApplicationIdentification workshopAppId;
    private byte[] ICC;
    private byte[] IC;

    public byte[] generateWorkshopCardDump(CardChannel channel) throws CardException, IOException, InstantiationException {
        ByteArrayOutputStream localByteArrayOutputStream = new ByteArrayOutputStream();

        int i = 1;
       
        if (Utilities.selectFile(2, channel)) {
          this.ICC = Utilities.readBlock(0, 25, channel);
        }

        if (Utilities.selectFile(5, channel)) {
          this.IC = Utilities.readBlock(0, 8, channel);
        } 
        
        if (Utilities.selectApplication(channel) && Utilities.selectFile(1281, channel)) {
          byte[] arrayOfByte = Utilities.readBlock(0, 1, channel);
          ApplicationIdentificationBase.CardType localCardType = ApplicationIdentificationBase.checkCardType(arrayOfByte);

          if (localCardType == ApplicationIdentificationBase.CardType.WORKSHOP) {
            this.workshopAppId = new WorkshopApplicationIdentification(Utilities.readBlock(0, 11, channel));
          }
        }
        
        writeUnsignedData(2, this.ICC, localByteArrayOutputStream);

        writeUnsignedData(5, this.IC, localByteArrayOutputStream);

        writeSignedDataFile(1281, 11, localByteArrayOutputStream, channel);

        writeUnsignedDataFile(49408, 194, localByteArrayOutputStream, channel);

        writeUnsignedDataFile(49416, 194, localByteArrayOutputStream, channel);

        int[] arrayOfInt1 = {1312, 1290, 1282, 1283, 1284, 1285, 1286, 1287, 1288, 1314};

        int[] arrayOfInt2 = {211, this.workshopAppId.getNumberOfCalibrationRecords() * 105 + 3, 432, 288, this.workshopAppId.getActivityStructureLength() + 4, this.workshopAppId.getNumberOfCardVehicleRecords() * 31 + 2, this.workshopAppId.getNumberOfPlaceRecords() * 10 + 1, 19, 46, 10};

        for (int j = 0; j < arrayOfInt1.length; j++) {
            writeSignedDataFile(arrayOfInt1[j], arrayOfInt2[j], localByteArrayOutputStream, channel);
            localByteArrayOutputStream.close();
        }
        try {
            resetCalibrationCounter(channel);
        } catch (CardException ex) {
            Logger.getLogger(CardDump.class.getName()).log(Level.SEVERE, null, ex);
        }
        return localByteArrayOutputStream.toByteArray();
    }

    private boolean resetCalibrationCounter(CardChannel channel) throws CardException {
        if (Utilities.selectFile(1289, channel)) {
            byte[] arrayOfByte = {0, 0};
            CommandAPDU localCommandAPDU = new CommandAPDU(0, 214, 0, 0, arrayOfByte);

            ResponseAPDU localResponseAPDU = channel.transmit(localCommandAPDU);
            int i = localResponseAPDU.getSW();
            return i == 36864;
        }
        return false;
    }

    private boolean writeUnsignedDataFile(int paramInt1, int paramInt2, ByteArrayOutputStream paramByteArrayOutputStream, CardChannel channel)
            throws CardException, IOException {
        if (Utilities.selectFile(paramInt1, channel)) {
            byte[] arrayOfByte1 = Utilities.readBlock(0, paramInt2, channel);
            if (arrayOfByte1 != null) {
                byte[] arrayOfByte2 = createFileHeader(paramInt1, paramInt2);
                try {
                    paramByteArrayOutputStream.write(arrayOfByte2);
                    paramByteArrayOutputStream.write(arrayOfByte1);
                    return true;
                } catch (IOException localIOException) {
                }
            }
        }
        return false;
    }

    private byte[] createFileHeader(int paramInt1, int paramInt2) {
        byte[] arrayOfByte = new byte[5];
        arrayOfByte[0] = Utilities.hiByte(paramInt1);
        arrayOfByte[1] = Utilities.lowByte(paramInt1);
        arrayOfByte[2] = 0;
        arrayOfByte[3] = Utilities.hiByte(paramInt2);
        arrayOfByte[4] = Utilities.lowByte(paramInt2);
        return arrayOfByte;
    }

    private boolean writeSignedDataFile(int paramInt1, int paramInt2, ByteArrayOutputStream paramByteArrayOutputStream, CardChannel channel)
            throws CardException, IOException {
        if ((Utilities.selectFile(paramInt1, channel)) && (performHash(channel))) {
            byte[] arrayOfByte1 = Utilities.readBlock(0, paramInt2, channel);
            byte[] arrayOfByte2 = computeDigitalSignature(channel);
            if ((arrayOfByte1 != null) && (arrayOfByte2 != null)) {
                byte[] arrayOfByte3 = createFileHeader(paramInt1, paramInt2);
                byte[] arrayOfByte4 = createSignatureHeader(paramInt1);
                try {
                    paramByteArrayOutputStream.write(arrayOfByte3);
                    paramByteArrayOutputStream.write(arrayOfByte1);
                    paramByteArrayOutputStream.write(arrayOfByte4);
                    paramByteArrayOutputStream.write(arrayOfByte2);
                    return true;
                } catch (IOException localIOException) {
                }
            }
        }
        return false;
    }

    private byte[] createSignatureHeader(int paramInt) {
        byte[] arrayOfByte = new byte[5];
        arrayOfByte[0] = Utilities.hiByte(paramInt);
        arrayOfByte[1] = Utilities.lowByte(paramInt);
        arrayOfByte[2] = 1;
        arrayOfByte[3] = Utilities.hiByte(128);
        arrayOfByte[4] = Utilities.lowByte(128);
        return arrayOfByte;
    }

    private boolean performHash(CardChannel channel) throws CardException {
        CommandAPDU localCommandAPDU = new CommandAPDU(128, 42, 144, 0);
        ResponseAPDU localResponseAPDU = channel.transmit(localCommandAPDU);
        int i = localResponseAPDU.getSW();
        return i == 36864;
    }

    private byte[] computeDigitalSignature(CardChannel channel) throws CardException {
        ByteArrayOutputStream localByteArrayOutputStream = new ByteArrayOutputStream(128);
        CommandAPDU localCommandAPDU = new CommandAPDU(0, 42, 158, 154, 128);

        ResponseAPDU localResponseAPDU = channel.transmit(localCommandAPDU);

        int i = localResponseAPDU.getSW();
        if (i == 36864) {
            try {
                localByteArrayOutputStream.write(localResponseAPDU.getData());
            } catch (IOException localIOException) {
                return null;
            }
            return localByteArrayOutputStream.toByteArray();
        }
        return null;
    }

    private boolean writeUnsignedData(int paramInt, byte[] paramArrayOfByte, ByteArrayOutputStream paramByteArrayOutputStream) {
        byte[] arrayOfByte = createFileHeader(paramInt, paramArrayOfByte.length);
        try {
            paramByteArrayOutputStream.write(arrayOfByte);
            paramByteArrayOutputStream.write(paramArrayOfByte);
            return true;
        } catch (IOException localIOException) {
            
        }
        return false;
    }
}