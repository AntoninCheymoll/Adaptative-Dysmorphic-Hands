using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandComponentManager;
using static HandTransformation;
using static InteractableManager;

public class SystemManager : MonoBehaviour
{


    public enum TransitionMode { Command, Context, Mix}
    public enum MorphingMode { Instant, Gradual}


    [Header("Main")]
    public TransitionMode transition_mode = TransitionMode.Context;
    public MorphingMode morphic_mode = MorphingMode.Instant;

    [Header("Specific")]
    public InteractionMode command_interaction_left = InteractionMode.normal;
    public InteractionMode command_interaction_right = InteractionMode.normal;

    public float morphing_gradual_duration = 0.5f;

    TransitionMode prev_transition_mode = TransitionMode.Context;
    public bool didtransitionModeChanged = false;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        didtransitionModeChanged = (prev_transition_mode != transition_mode);
        
        if(didtransitionModeChanged)
        {
            command_interaction_left = InteractionMode.normal;
            command_interaction_right = InteractionMode.normal;
        }

        prev_transition_mode = transition_mode;
    }

    public InteractionMode getCurrentCommandInteraction(Handness handness)
    {
        return (handness == Handness.left)? command_interaction_left : command_interaction_right;
    }

    public void SetCurrentCommandInteraction(InteractionMode inter, Handness handness)
    {
        if (handness == Handness.left) command_interaction_left = inter;
        if (handness == Handness.right) command_interaction_right = inter;
    }

    
}
