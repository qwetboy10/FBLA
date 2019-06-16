using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * ArgumentException is a subclass of Exception for debugging purposes.
 * It is specifically thrown when the debug console is given invalid arguments,
 * so specific catching of ArgumentException is possible (ArgumentExceptions should not
 * stop the game, but others should).
 */

//extends Exception to allow for all Exception related usages
public class ArgumentException : Exception {
	
}
