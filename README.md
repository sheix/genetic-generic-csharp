genetic-generic-csharp
======================

Started as genetic algorithm implementation for some simple DSP task, just as POC, it evolved to be genetic algorithm engine, that slightly different from traditional definition of genetic algorithm, but:
* easy to use and configure
* designed with extensibility in mind
* using object representation instead of binary strings, thus generic to any c# class

The more detailed description will follow

#How to use it:

* obtain a dll (clone or download release) 
* reference genetic-generic-engine
* create an Engine object with EngineParameters (fill all fields in)
* run methods RunIteration() or RunIterations()
* take a look on result with GetBest() method

Feel free to contact me or open an issues :)
