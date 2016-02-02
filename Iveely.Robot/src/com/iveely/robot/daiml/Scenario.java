/**
 * date   : 2016年2月2日
 * author : Iveely Liu
 * contact: sea11510@mail.ustc.edu.cn
 */
package com.iveely.robot.daiml;

import java.util.ArrayList;
import java.util.List;

import org.dom4j.Element;

/**
 * @author {Iveely Liu}
 *
 */
public class Scenario {

	/**
	 * Expression of scenario.
	 */
	private String express;

	/**
	 * Index id of star.
	 */
	private List<Integer> ids;

	public Scenario() {
		this.ids = new ArrayList<>();
	}

	/**
	 * Parse element to script.
	 * 
	 * @param element
	 * @return
	 */
	public boolean parse(Element element) {
		List<Element> children = element.elements();
		for (Element child : children) {
			String tag = child.getName();
			if (tag.equals("star")) {
				int id = Integer.parseInt(child.attributeValue("index"));
				this.ids.add(id);
				child.setText("%s" + id + "%");
			} else if (tag.equals("ret")) {
				child.setText("%r%");
			} else {
				return false;
			}
		}
		this.express = element.getStringValue().trim();
		return true;
	}

	/**
	 * Get script content.
	 * 
	 * @param stars
	 * @param ret
	 * @return
	 */
	public String getScript(List<String> stars, String ret) {
		String result = express.replace("%r%", ret);
		for (Integer id : ids) {
			result = result.replace("%s" + id + "%", stars.get(id - 1));
		}
		return result;
	}

}
