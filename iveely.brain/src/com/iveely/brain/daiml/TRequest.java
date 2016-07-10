/**
 * date   : 2016年1月29日 author : Iveely Liu contact: sea11510@mail.ustc.edu.cn
 */
package com.iveely.brain.daiml;

import com.iveely.brain.environment.Script;
import com.iveely.brain.mind.Brain;
import com.iveely.brain.mind.Idio;
import com.iveely.brain.mind.React.Status;
import com.iveely.framework.net.Packet;

import org.dom4j.Element;

import java.util.ArrayList;
import java.util.List;

/**
 * @author {Iveely Liu}
 */
public class TRequest extends ITemplate {

  /**
   * Requst information.
   */
  private List<BranchNode> requests;

  /**
   * Response detail.
   */
  private Ret ret;

  /**
   * Script of the response.
   */
  private Scenario script;

  public TRequest() {
    this.requests = new ArrayList<>();
  }

  /*
   * (non-Javadoc)
   *
   * @see com.iveely.robot.daiml.ITemplate#parse(org.dom4j.Element)
   */
  public boolean parse(Element element) {
    List<Element> children = element.elements();
    if (children.size() == 0) {
      return false;
    }
    for (Element ele : children) {
      String tag = ele.getName().toLowerCase();
      // 1. Parse request.
      if (tag.equals("request")) {
        BranchNode req = new BranchNode();
        if (req.parse(ele)) {
          requests.add(req);
        } else {
          return false;
        }

      }
      // 2. Parse ret.
      else if (tag.equals("ret")) {
        ret = new Ret();
        if (!ret.parse(ele)) {
          return false;
        }
      }
      // 3. Parse script.
      else if (tag.equals("script")) {
        script = new Scenario();
        if (!script.parse(ele)) {
          return false;
        }
      }
      // 4. Unknown.
      else {
        return false;
      }
    }
    return true;
  }

  /*
   * (non-Javadoc)
   *
   * @see com.iveely.robot.daiml.ITemplate#getType()
   */
  public Status getStatus() {
    return Status.SUCCESS;
  }

  /*
   * (non-Javadoc)
   *
   * @see com.iveely.robot.daiml.ITemplate#getResult()
   */
  public String getResult(List<String> stars, String that) {
    // 1. Get parameter of request.
    List<String> nodes = new ArrayList<>();
    for (BranchNode req : requests) {
      String par = req.getParameter(stars);
      Packet packet = Brain.getInstance().getBranch(req.getName()).send(par);
      String temp = com.iveely.framework.text.StringUtil.getString(packet.getData());
      nodes.add(temp);
    }

    // 2. Replace ret.
    String retText = ret.getContent(stars, nodes);

    // 3. Check is recursion.
    String result = retText;
    if (ret.getStatus() == Status.RECURSIVE) {
      Idio idio = Brain.getInstance().think(retText, that);
      if (idio == null) {
        result = null;
      } else {
        result = idio.getResponse();
      }
    }

    // 4. Execute script.
    if (script == null || result == null) {
      return result;
    } else {
      String sret = script.getScript(stars, result, nodes).trim();
      return Script.eval(sret);
    }
  }

}
