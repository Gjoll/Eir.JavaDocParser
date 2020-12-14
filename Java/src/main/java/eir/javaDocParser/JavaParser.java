package eir.javaDocParser;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;

import com.thoughtworks.qdox.JavaDocBuilder;
import eir.javaDocParser.FileSearch;

public class JavaParser {
    public void ParseDir(String dir) {
        FileSearch fs = new FileSearch();
        fs.searchDirectory(new File(dir), ".*?.java");

        int count = fs.getResult().size();
        System.out.println("\nFound " + count + " result!\n");
        for (String matched : fs.getResult())
            ParseFile(matched);
    }

    public void ParseFile(String fileFullPath)  {
        System.out.println("Parsing : " + fileFullPath);

        try {
            JavaDocBuilder builder = new JavaDocBuilder();
            builder.addSource(new FileReader(fileFullPath));
        } catch (Exception e)
        {
            System.out.println("Error Parsing : " + fileFullPath + ". " + e.getMessage());
        }
    }
}
