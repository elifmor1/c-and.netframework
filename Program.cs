using System;

// Main program class that serves as the entry point
class Program
{
    static void Main(string[] args)
    {
        // Create and run the shipping quote state machine
        var stateMachine = new ShippingQuoteStateMachine();
        stateMachine.Start();
    }
}

// Base class for all shipping quote states
abstract class ShippingQuoteState
{
    protected ShippingQuoteStateMachine StateMachine;

    public void SetContext(ShippingQuoteStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public abstract void Process();
}

// State for handling initial welcome message
class WelcomeState : ShippingQuoteState
{
    public override void Process()
    {
        Console.WriteLine("Welcome to Package Express. Please follow the instructions below.");
        StateMachine.TransitionTo(new WeightInputState());
    }
}

// State for handling weight input and validation
class WeightInputState : ShippingQuoteState
{
    public override void Process()
    {
        Console.WriteLine("Please enter the package weight:");
        if (double.TryParse(Console.ReadLine(), out double weight))
        {
            if (weight > 50)
            {
                Console.WriteLine("Package too heavy to be shipped via Package Express. Have a good day.");
                StateMachine.TransitionTo(new EndState());
            }
            else
            {
                StateMachine.PackageWeight = weight;
                StateMachine.TransitionTo(new DimensionsInputState());
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a numeric value.");
            Process();
        }
    }
}

// State for handling dimensions input and validation
class DimensionsInputState : ShippingQuoteState
{
    private int _dimensionStep = 0;
    private double _width, _height, _length;

    public override void Process()
    {
        switch (_dimensionStep)
        {
            case 0:
                GetWidth();
                break;
            case 1:
                GetHeight();
                break;
            case 2:
                GetLength();
                break;
        }
    }

    private void GetWidth()
    {
        Console.WriteLine("Please enter the package width:");
        if (double.TryParse(Console.ReadLine(), out _width))
        {
            _dimensionStep++;
            Process();
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a numeric value.");
            GetWidth();
        }
    }

    private void GetHeight()
    {
        Console.WriteLine("Please enter the package height:");
        if (double.TryParse(Console.ReadLine(), out _height))
        {
            _dimensionStep++;
            Process();
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a numeric value.");
            GetHeight();
        }
    }

    private void GetLength()
    {
        Console.WriteLine("Please enter the package length:");
        if (double.TryParse(Console.ReadLine(), out _length))
        {
            if (_width + _height + _length > 50)
            {
                Console.WriteLine("Package too big to be shipped via Package Express.");
                StateMachine.TransitionTo(new EndState());
            }
            else
            {
                StateMachine.PackageDimensions = (_width, _height, _length);
                StateMachine.TransitionTo(new QuoteCalculationState());
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a numeric value.");
            GetLength();
        }
    }
}

// State for calculating and displaying the shipping quote
class QuoteCalculationState : ShippingQuoteState
{
    public override void Process()
    {
        var (width, height, length) = StateMachine.PackageDimensions;
        var quote = (width * height * length * StateMachine.PackageWeight) / 100;
        
        Console.WriteLine($"Your estimated total for shipping this package is: ${quote:F2}");
        Console.WriteLine("Thank you!");
        
        StateMachine.TransitionTo(new EndState());
    }
}

// Final state indicating the end of the process
class EndState : ShippingQuoteState
{
    public override void Process()
    {
        // Process ends here
    }
}

// Main state machine class that manages state transitions
class ShippingQuoteStateMachine
{
    private ShippingQuoteState _currentState;
    public double PackageWeight { get; set; }
    public (double width, double height, double length) PackageDimensions { get; set; }

    public void Start()
    {
        TransitionTo(new WelcomeState());
        _currentState.Process();
    }

    public void TransitionTo(ShippingQuoteState state)
    {
        _currentState = state;
        _currentState.SetContext(this);
        if (!(state is EndState))
        {
            _currentState.Process();
        }
    }
}