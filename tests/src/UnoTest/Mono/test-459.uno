namespace Mono.test_459
{
    using Uno;
    
    class CC {
        
        public class IfElseStateMachine {
                
                public enum State {
                START,
                IF_SEEN,
                ELSEIF_SEEN,
                ELSE_SEEN,
                ENDIF_SEEN,
                MAX
                }
            
                public enum Token {
                START,
                IF,
                ELSEIF,
                ELSE,
                ENDIF,
                EOF,
                MAX
                }
    
                State state;
                public IfElseStateMachine()
                {
                }
    
                public void HandleToken(Token tok)
                {    
                    if(tok == Token.IF) {
                        state = (State) tok;
                    }
                }
            }
            
            [Uno.Testing.Test] public static void test_459() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() 
            {
                new IfElseStateMachine ().HandleToken (IfElseStateMachine.Token.IF);
                return 0;
            }
    }
}
