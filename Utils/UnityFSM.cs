using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFSM
{
	/// <summary>
	/// This class represents the States in the Finite State System.
	/// Each state has a Dictionary with pairs (transition-state) showing
	/// which state the FSM should be if a transition is fired while this state
	/// is the current state.
	/// Method Reason is used to determine which transition should be fired .
	/// Method Act has the code to perform the actions the NPC is supposed do if it's on this state.
	/// </summary>
	public abstract class FSMState
	{
		protected List<int> transitionTo = new List<int>();
	    protected int stateID;
	    public int ID { get { return stateID; } }
	 
	    public void AddTransition(int stateId)
		{

			if (stateId == 0)
			{
				Debug.LogError("FSMState ERROR: NullStateID is not allowed for a real ID");
				return;
			}

			if (transitionTo.Contains(stateId))
				Debug.LogWarning("FSMState WARNING: State already has a transtition to " + stateId);
			else
				transitionTo.Add(stateId);
	    }
	 
	    /// <summary>
	    /// This method deletes a pair transition-state from this state's map.
	    /// If the transition was not inside the state's map, an ERROR message is printed.
	    /// </summary>
	    public void DeleteTransition(int transitionToId)
	    {
			if (transitionTo.Contains(transitionToId))
				transitionTo.Remove(transitionToId);
			else
				Debug.LogError("FSMState Error : " + stateID + " does not transition to " + transitionToId);
	    }

		public bool CanTransitionTo(int destinationStateId)
		{
			return transitionTo.Contains(destinationStateId);
		}
	 
	    /// <summary>
	    /// This method is used to set up the State condition before entering it.
	    /// It is called automatically by the FSMSystem class before assigning it
	    /// to the current state.
	    /// </summary>
	    public virtual void DoBeforeEntering() { }
	 
	    /// <summary>
	    /// This method is used to make anything necessary, as reseting variables
	    /// before the FSMSystem changes to another one. It is called automatically
	    /// by the FSMSystem before changing to a new state.
	    /// </summary>
	    public virtual void DoBeforeLeaving() { } 
	 
	    /// <summary>
	    /// This method decides if the state should transition to another on its list
	    /// NPC is a reference to the object that is controlled by this class
	    /// </summary>
		public virtual void SwitchLogic() { }
	 
	    /// <summary>
	    /// This method controls the behavior of the NPC in the game World.
	    /// Every action, movement or communication the NPC does should be placed here
	    /// NPC is a reference to the object that is controlled by this class
	    /// </summary>
	    public virtual void Update() { }

		public virtual void LateUpdate() { }
	 
	} // class FSMState
	 
	 
	/// <summary>
	/// FSMSystem class represents the Finite State Machine class.
	///  It has a List with the States the NPC has and methods to add,
	///  delete a state, and to change the current state the Machine is on.
	/// </summary>
	public class FSMSystem
	{
	    private List<FSMState> states;
	 
	    // The only way one can change the state of the FSM is by performing a transition
	    // Don't change the CurrentState directly
		public FSMState CurrentState { get; protected set; }
		public int CurrentStateID { get { return CurrentState.ID; } }
	 
	    public FSMSystem()
	    {
	        states = new List<FSMState>();
	    }
	 
	    /// <summary>
	    /// This method places new states inside the FSM,
	    /// or prints an ERROR message if the state was already inside the List.
	    /// First state added is also the initial state.
	    /// </summary>
	    public void AddState(FSMState s)
	    {
	        // Check for Null reference before deleting
	        if (s == null)
	        {
	            Debug.LogError("FSM ERROR: Null reference is not allowed");
	        }
	 
	        // First State inserted is also the Initial state,
	        //   the state the machine is in when the simulation begins
	        if (states.Count == 0)
	        {
	            states.Add(s);
	            CurrentState = s;
	            return;
	        }
	 
	        // Add the state to the List if it's not inside it
	        foreach (FSMState state in states)
	        {
	            if (state.ID == s.ID)
	            {
	                Debug.LogError("FSM ERROR: Impossible to add state " + s.ID.ToString() + 
	                               " because state has already been added");
	                return;
	            }
	        }
	        states.Add(s);
	    }

		public FSMState GetState(int stateId)
		{
			if (stateId == 0)
			{
				Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
				return null;
			}

			FSMState outState = null;
			foreach (FSMState checkState in states)
			{
				if (checkState.ID == stateId)
					outState = checkState;
			}

			if (outState == null)
				Debug.LogError("FSM Error: Requested state " + stateId + " not found");

			return outState;
		}
	 
	    /// <summary>
	    /// This method delete a state from the FSM List if it exists, 
	    ///   or prints an ERROR message if the state was not on the List.
	    /// </summary>
	    public void DeleteState(int stateId)
	    {
	        // Check for NullState before deleting
	        if (stateId == 0)
	        {
	            Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
	            return;
	        }
	 
	        // Search the List and delete the state if it's inside it
	        foreach (FSMState state in states)
	        {
	            if (state.ID == stateId)
	            {
	                states.Remove(state);
	                return;
	            }
	        }
	        Debug.LogError("FSM ERROR: Impossible to delete state " + stateId.ToString() + 
	                       ". It was not on the list of states");
	    }
		
		public virtual bool ChangeState(int stateId)
		{
			if (stateId == 0)
				return false;

			if (CurrentState.CanTransitionTo(stateId))
			{
				foreach (FSMState state in states)
				{
					if (state.ID == stateId)
					{
						// Do the post processing of the state before setting the new one
						CurrentState.DoBeforeLeaving();

						CurrentState = state;

						// Reset the state to its desired condition before it can reason or act
						CurrentState.DoBeforeEntering();
						return true;
					}
				}
			}
			Debug.LogError("FSM ERROR: Couldn't change to state " + stateId + " from state " + CurrentState.ID);
			return false;
		}
	 
	} //class FSMSystem
}

