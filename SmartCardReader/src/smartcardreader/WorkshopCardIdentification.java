package smartcardreader;

import java.io.UnsupportedEncodingException;
import java.util.Date;

public class WorkshopCardIdentification {

    private static final long serialVersionUID = 1L;
    public static final int RECORD_SIZE = 211;
    private final String cardSerialNumber;
    private Date cardExpiryDate;
    private final String workshopName;

    public WorkshopCardIdentification(byte[] paramArrayOfByte) throws UnsupportedEncodingException {

        ByteArrayReader localByteArrayReader = new ByteArrayReader(paramArrayOfByte);

        localByteArrayReader.read(1);

        this.cardSerialNumber = new String(localByteArrayReader.read(16), "US-ASCII").trim();

        localByteArrayReader.read(36);

        localByteArrayReader.read(4);

        localByteArrayReader.read(4);

        this.cardExpiryDate = new Date(Utilities.toLong(localByteArrayReader.read(4)) * 1000L);

        this.workshopName = new String(localByteArrayReader.read(36), "US-ASCII").trim();
    }

    public String getWorkshopName() {
        return this.workshopName;
    }

    public String getWorkshopCardSerialNumber() {
        return this.cardSerialNumber;
    }

    public Date getWorkshopCardExpiryDate() {
        return this.cardExpiryDate;
    }
}
