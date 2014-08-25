package smartcardreader;

public class WorkshopApplicationIdentification extends ApplicationIdentificationBase {

    private static final long serialVersionUID = 1L;
    public static final int RECORD_SIZE = 11;
    private int numberOfCalibrationRecords;

    public WorkshopApplicationIdentification(byte[] paramArrayOfByte)
            throws InstantiationException {
        if (paramArrayOfByte.length != 11) {
            throw new InstantiationException("Application identification block has invalid size");
        }

        ByteArrayReader localByteArrayReader = new ByteArrayReader(paramArrayOfByte);
        Object localObject1 = null;
        try {
            this.cardType = ApplicationIdentificationBase.CardType.findType(localByteArrayReader.read());

            this.cardStructureVersion = Utilities.toInt(localByteArrayReader.read(2));

            this.numberOfEventsperType = localByteArrayReader.read();
            this.numberOfFaultsPerType = localByteArrayReader.read();

            this.activityStructureLength = Utilities.toInt(localByteArrayReader.read(2));

            this.numberOfCardVehicleRecords = Utilities.toInt(localByteArrayReader.read(2));

            this.numberOfPlaceRecords = localByteArrayReader.read();
            this.numberOfCalibrationRecords = localByteArrayReader.read();
        } catch (Throwable localThrowable2) {
            localObject1 = localThrowable2;
            throw localThrowable2;
        }
    }

    public int getNumberOfCalibrationRecords() {
        return this.numberOfCalibrationRecords;
    }
}