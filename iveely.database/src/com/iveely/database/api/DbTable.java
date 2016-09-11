package com.iveely.database.api;

import com.iveely.database.storage.Types;

import java.util.ArrayList;
import java.util.List;

/**
 * @author liufanping@iveely.com
 */
public class DbTable {

  private final List<DbFiled> fileds;
  private String name;

  public DbTable() {
    this.fileds = new ArrayList<>();
  }

  /**
   * @return the name
   */
  public String getName() {
    return name;
  }

  /**
   * @param name the name to set
   */
  public void setName(String name) {
    this.name = name;
  }

  /**
   * Add filed.
   * @param name filed name
   * @param type type of filed
   * @param isUnique set unique
   * @return whether add success
   */
  public boolean addFiled(String name, Types type, boolean isUnique) {
    DbFiled filed = new DbFiled();
    filed.setIsUnique(isUnique);
    filed.setName(name);
    return true;
  }
}
