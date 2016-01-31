/**
 * date   : 2016年1月29日
 * author : Iveely Liu
 * contact: sea11510@mail.ustc.edu.cn
 */
package com.iveely.robot.daiml;

import java.util.List;

import org.dom4j.Element;

import com.iveely.robot.mind.React.Status;

/**
 * @author {Iveely Liu}
 *
 */
public class NormalTemplate implements ITemplate {

	/**
	 * Text of the answer.
	 */
	private String text;

	/**
	 * Type of the template.
	 */
	private Status status;

	public NormalTemplate() {
		status = Status.SUCCESS;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see com.iveely.robot.daiml.ITemplate#getType()
	 */
	@Override
	public Status getStatus() {
		return status;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see com.iveely.robot.daiml.ITemplate#getValue()
	 */
	@Override
	public String getResult(List<String> stars) {
		String ret = text;
		for (int i = stars.size() - 1; i > -1; i--) {
			ret = ret.replace("%s+" + (i + 1) + "%", stars.get(i));
		}
		return ret;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see com.iveely.robot.daiml.ITemplate#parse(org.dom4j.Element)
	 */
	@Override
	public boolean parse(Element element) {
		this.text = element.asXML().trim().replace("<template>", "").replace("</template>", "");
		return true;
	}
}
