/**
 * date   : 2016年1月31日
 * author : Iveely Liu
 * contact: sea11510@mail.ustc.edu.cn
 */
package com.iveely.robot.api;

import com.iveely.robot.environment.Variable;
import com.iveely.robot.mind.Awareness;
import com.iveely.robot.mind.Brain;
import com.iveely.robot.net.websocket.SocketClient;
import com.iveely.robot.net.websocket.WSHandler;

import groovy.transform.Synchronized;

/**
 * @author {Iveely Liu}
 *
 */
public class Local {

	/**
	 * Whether identification has been started.
	 */
	private boolean isStarted;

	public Local() {
		isStarted = false;
		Variable.setLocal();
	}

	/**
	 * Local service start.
	 */
	public void start() {
		if (!isStarted) {
			synchronized (Local.class) {
				if (!isStarted) {
					Awareness.wake();
					isStarted = true;
				}
			}
		}
	}

	/**
	 * Start
	 * 
	 * @param msg
	 * @return
	 */
	public String send(String msg) {
		if (!isStarted) {
			start();
		}
		return Brain.getInstance().think(msg);
	}

}
