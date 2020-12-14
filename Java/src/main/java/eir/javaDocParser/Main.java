package eir.javaDocParser;

public class Main {

    static JavaParser parser = new JavaParser();

    static void parseArgs(String[] args) throws Exception {
        int i = 0;
        while (i < args.length)
        {
            String arg = args[i++];
            switch (arg)
            {
                case "-d":
                    String dir = args[i++];
                    parser.ParseDir(dir);
                    break;

                default:
                    throw new Exception("Invalid command line option");
            }
        }
    }

    public static void main(String[] args) {
        try {
            Main.parseArgs(args);
        } catch(Exception e)
        {
            System.out.print("Error: " + e.getMessage());
        }
    }
}
