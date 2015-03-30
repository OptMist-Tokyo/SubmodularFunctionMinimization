#include "Library.h"

using namespace OnigiriLibrary;


string Converter::IntToString(long number)
{
	stringstream ss;
	ss << number;
	return ss.str();
}

string Converter::DoubleToString(double number){
	std::ostringstream oss;
	oss <<number;
	 string res( oss.str() ); 
	 return res;
}