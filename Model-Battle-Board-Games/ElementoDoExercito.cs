using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;



namespace BattleBoardGame.Model
{
    /// <summary>
    /// Um elemento do exército. É uma classe abstrata. Significa que somente 
    /// as subclasses concretas que herdam desta classe poderão ser instanciadas.
    /// </summary>
    [DataContract(IsReference = true)]
    public abstract class ElementoDoExercito
    {
        /// <summary>
        /// Determina se o objeto especificado <see cref="object"/> é igual ao objeto <see cref="T:BattleBoardGame.Model.ElementoDoExercito"/>.
        /// </summary>
        /// <param name="obj">O <see cref="object"/> a ser comparado ao objeto <see cref="T:BattleBoardGame.Model.ElementoDoExercito"/>.</param>
        /// <returns><c>true</c> Se os dois objetos forem <see cref="ElementoDoExercito"/> e os dois tiverem o mesmo Id então 
        /// irá retornar <c>true</c>, caso contrário <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ElementoDoExercito && this.Id > 0)
            {
                return ((ElementoDoExercito)obj).Id == this.Id;
            }
            return object.ReferenceEquals(this, obj);
        }

        /// <summary>
        /// A função de hash é utilizada para calcular a posição do item algumas coleções. <see cref="T:BattleBoardGame.Model.ElementoDoExercito"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            if (this.Id > 0)
            {
                return this.Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Saude { get; set; }
        [DataMember]
        public Posicao posicao { get; set; }

        [DataMember]
        public int TabuleiroId { get; set; }

        /// <summary>
        /// A anotação ForeignKey específica que o membro "TabuleiroId" desta classe
        /// armazena a chave estrangeira que faz referência a entidade Tabuleiro.
        /// Ter este tipo de propriedade explicita é útil evitar o salvamento 
        /// aninhado, que poderá trazer inúmeras complicações e/ou diminuir o tamanho
        /// do objeto a ser enviado, por exemplo em redes de dados de baixa 
        /// velocidade.
        /// </summary>
        /// <value>The tabuleiro.</value>
        [ForeignKey("TabuleiroId")]
        [InverseProperty("ElementosDoExercito")]
        public Tabuleiro Tabuleiro { get; set; }

        [DataMember]
        public int ExercitoId { get; set; }

        [ForeignKey("ExercitoId")]
        [InverseProperty("Elementos")]
        public virtual Exercito Exercito { get; set; }

        [DataMember]
        public abstract int AlcanceMovimento {get; protected set;}

        [DataMember]
        public abstract int AlcanceAtaque { get; protected set; }

        [DataMember]
        public abstract int Ataque { get; protected set; }

        [DataMember]

        public abstract string Imagem { get; protected set; }
    }


}
