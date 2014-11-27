package smartcardreader;

import java.io.BufferedOutputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.Iterator;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.smartcardio.CardChannel;
import javax.smartcardio.CardException;
import javax.smartcardio.CardTerminal;
import javax.smartcardio.CardTerminals;
import javax.smartcardio.TerminalFactory;

public class SmartCardReader {

    public static void main(String[] args) {
        try {
            CardTerminals localCardTerminals = TerminalFactory.getDefault().terminals();
            List<CardTerminal> localList = localCardTerminals.list(CardTerminals.State.CARD_INSERTION);

            for (CardTerminal localCardTerminal : localList) {
                javax.smartcardio.Card card = localCardTerminal.connect("T=1");

                CardChannel channel = card.getBasicChannel();

                if (args[0].equals("/calibrations")) {
                    ReadAllCalibrationRecords(channel);
                } else if (args[0].equals("/dump")) {
                    byte[] dump = new CardDump().generateWorkshopCardDump(channel);

                    String tempPath = System.getProperty("java.io.tmpdir") + "dump.ddd";
                    if (Utilities.selectFile(1312, channel)) {
                        WorkshopCardIdentification workshopCardId = new WorkshopCardIdentification(Utilities.readBlock(0, 211, channel));
                        tempPath = System.getProperty("java.io.tmpdir") + GenerateFileName(workshopCardId);

                        WriteWorkshopData(workshopCardId, tempPath);
                        try (BufferedOutputStream bos = new BufferedOutputStream(new FileOutputStream(tempPath))) {
                            bos.write(dump);
                            bos.flush();
                        }
                    }
                }
            }
        } catch (Exception ex) {
            Logger.getLogger(SmartCardReader.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    public static void GetLatestCalibrationRecord(CardChannel channel) {
        try {
            if ((Utilities.selectApplication(channel)) && (Utilities.selectFile(1290, channel))) {
                int i = Utilities.toInt(Utilities.readBlock(2, 1, channel));

                byte[] arrayOfByte = Utilities.readBlock(3 + i * 105, 105, channel);

                List<CalibrationRecord> records = new ArrayList();

                CalibrationRecord calibrationRecord = new CalibrationRecord(arrayOfByte);

                if (Utilities.selectFile(1312, channel)) {
                    WorkshopCardIdentification workshopCardId = new WorkshopCardIdentification(Utilities.readBlock(0, 211, channel));
                    calibrationRecord.setCardSerialNumber(workshopCardId.getWorkshopCardSerialNumber());
                }

                records.add(calibrationRecord);

                System.out.println(AllCalibrationsAsXML(records));
            }
        } catch (CardException | IOException ex) {
            Logger.getLogger(SmartCardReader.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    public static void ReadAllCalibrationRecords(CardChannel channel) {
        try {
            if ((Utilities.selectApplication(channel)) && (Utilities.selectFile(1290, channel))) {
                int i = Utilities.toInt(Utilities.readBlock(0, 2, channel));

                List<CalibrationRecord> calibrationRecords = new ArrayList();
                for (int j = 0; j < i; j++) {
                    byte[] arrayOfByte = Utilities.readBlock(3 + j * 105, 105, channel);

                    CalibrationRecord localCalibrationRecord = new CalibrationRecord(arrayOfByte);

                    calibrationRecords.add(localCalibrationRecord);
                }

                System.out.println(AllCalibrationsAsXML(calibrationRecords));
            }
        } catch (CardException | IOException ex) {
            System.out.println("Error: " + ex.getMessage());
        }
    }

    private static String AllCalibrationsAsXML(List<CalibrationRecord> calibrationRecords) {
        StringBuilder sb = new StringBuilder();

        sb.append("<?xml version='1.0' encoding='ISO-8859-1'?>");
        sb.append("<CalibrationRecords>");

        for (Iterator<CalibrationRecord> i = calibrationRecords.iterator(); i.hasNext();) {
            CalibrationRecord item = i.next();
            sb.append(item.toXML());
        }

        sb.append("</CalibrationRecords>");
        return sb.toString();
    }

    private static String GenerateFileName(WorkshopCardIdentification workshopCardId) {
        StringBuilder sb = new StringBuilder();

        Date today = new Date();
        SimpleDateFormat format = new SimpleDateFormat("yyyyMMdd_HHmm");
        String date = format.format(today);

        sb.append("C_");
        sb.append(date);
        sb.append("_");
        sb.append(workshopCardId.getWorkshopName());
        sb.append("_");
        sb.append(workshopCardId.getWorkshopCardSerialNumber());
        sb.append(".DDD");

        return sb.toString();
    }

    private static void WriteWorkshopData(WorkshopCardIdentification workshopCardId, String tempPath) {
        System.out.print("<?xml version='1.0' encoding='ISO-8859-1'?>");
        System.out.print("<CardInformation>");
        System.out.print("<CardDump>");
        System.out.print("<WorkshopName>" + workshopCardId.getWorkshopName() + "</WorkshopName>");
        System.out.print("<TempPath>" + tempPath + "</TempPath>");
        System.out.print("</CardDump>");
        System.out.print("</CardInformation>");
    }
}