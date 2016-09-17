package com.iveely.brain;

import com.iveely.brain.api.Local;
import com.iveely.brain.mind.Awareness;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URL;

public class BrainApplication {

  public static void main(String[] args) throws IOException {
    if (args != null && args.length > 1) {
      Awareness.wake();
    } else {
      Local local = new Local();
      local.start();
      BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
      while (true) {
        try {
          System.out.print("Q:");
          String text = reader.readLine().trim();
          if (text.equals("exit")) {
            break;
          }
          System.out.println("A:" + local.send(text));
        } catch (IOException ex) {
          System.err.println(ex);
        }
      }
      reader.close();
    }
  }
}
