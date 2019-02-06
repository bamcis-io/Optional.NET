using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BAMCIS.Util
{
    /// <summary>
    /// A container object which may or may not contain a non-null value.
    /// If a value is present, IsPresent() returns true and
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Optional<T> where T : class
    {
        #region Private Fields

        // Since we can possibly support structs and classes,
        // use this field to track whether the container is empty
        // since a struct can't be compared to null
        private bool _Empty;

        #endregion

        #region Public Properties

        /// <summary>
        /// If non-null, the value; if null, indicates no value is present
        /// </summary>
        public T Value { get; }

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Returns the Empty optional instance, no value is present for this
        /// </summary>
        public static Optional<T> Empty { get; } = new Optional<T>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an empty instance.
        /// </summary>
        private Optional()
        {
            this.Value = default(T);
            this._Empty = true;
        }

        /// <summary>
        /// Constructs an instance with the described value.
        /// </summary>
        /// <param name="value"></param>
        public Optional(T value)
        {
            this.Value = value ?? throw new ArgumentNullException("value");
            this._Empty = false;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Returns an Optional describing the given non-null value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Optional<T> Of(T value)
        {
            return new Optional<T>(value);
        }

        /// <summary>
        /// Returns an Optional describing the given value, if non-null, otherwise
        /// returns an empty Optional
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Optional<T> OfNullable(T value)
        {
            return value == null ? Optional<T>.Empty : Optional<T>.Of(value);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// If a value is present, rturns true, otherwise false.
        /// </summary>
        /// <returns></returns>
        public bool IsPresent()
        {
            return !this._Empty;
        }

        /// <summary>
        /// If a value is present, performs the given action with the value, otherwise does nothing
        /// </summary>
        /// <param name="action"></param>
        public void IfPresent(Action<T> action)
        {
            if (this.IsPresent())
            {
                action.Invoke(this.Value);
            }
        }

        /// <summary>
        /// If a value is present, performs the given action with the value, otherwise performs the
        /// given empty-based action
        /// </summary>
        /// <param name="action"></param>
        public void IfPresentOrElse(Action<T> action, ThreadStart emptyAction)
        {
            if (this.IsPresent())
            {
                action.Invoke(this.Value);
            }
            else
            {
                Thread Thr = new Thread(emptyAction);
                Thr.Start();
                Thr.Join();
            }
        }

        /// <summary>
        /// If a value is present, and the value matches the given predicate,
        /// returns an Optional describing the value, otherwise returns an empty
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Optional<T> Filter(Predicate<T> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            if (!this.IsPresent())
            {
                return this;
            }
            else
            {
                return predicate.Invoke(this.Value) ? this : Optional<T>.Empty;
            }
        }

        /// <summary>
        /// If a value is present, returns an Optional describing the result of applying the given
        /// mapping function to the value, otherwise returns an empty.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public Optional<TResult> Map<TResult>(Func<T, TResult> mapper) where TResult : class
        {
            if (mapper == null)
            {
                throw new ArgumentNullException("mapper");
            }

            if (!this.IsPresent())
            {
                return new Optional<TResult>();
            }
            else
            {
                return Optional<TResult>.OfNullable(mapper.Invoke(this.Value));
            }
        }

        /// <summary>
        /// If a value is present, returns the result of applying the given Optional-bearing
        /// mapping function to the value, otherwise returns an empty.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public Optional<TResult> FlatMap<TResult>(Func<T, Optional<TResult>> mapper) where TResult : class
        {
            if (mapper == null)
            {
                throw new ArgumentNullException("mapper");
            }

            if (!this.IsPresent())
            {
                return new Optional<TResult>();
            }
            else
            {
                Optional<TResult> result = mapper.Invoke(this.Value);
                if (result == null)
                {
                    throw new InvalidOperationException("The mapper function provided a null result.");
                }
                else
                {
                    return result;
                }
            }
        }

        /// <summary>
        /// This method can be used to transform of optional elements to a Stream of present
        /// value elements.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Stream()
        {
            if (!this.IsPresent())
            {
                return Enumerable.Empty<T>();
            }
            else
            {
                return new List<T>() { this.Value };
            }
        }

        /// <summary>
        /// If a value is present, returns an Optional describing the value, otherwise
        /// returns an Optional produced by the supplying function.
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public Optional<T> Or(Func<Optional<T>> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException("supplier");
            }

            if (this.IsPresent())
            {
                return this;
            }
            else
            {
                Optional<T> Result = supplier.Invoke();

                if (Result == null)
                {
                    throw new InvalidOperationException("The supplier provided a null result.");
                }
                else
                {
                    return Result;
                }
            }
        }

        /// <summary>
        /// If a value is present, returns the value, otherwise returns other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public T OrElse(T other)
        {
            return this.IsPresent() ? this.Value : other;
        }

        /// <summary>
        /// If a value is present, returns the value, otherwise returns the result
        /// produced by the supplying function
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public T OrElseGet(Func<T> supplier)
        {
            return this.IsPresent() ? this.Value : supplier.Invoke();
        }

        /// <summary>
        /// If a value is present, returns the value, otherwise throws an exception
        /// produced by the exception supplying function
        /// </summary>
        /// <typeparam name="Ex"></typeparam>
        /// <param name="exceptionSupplier"></param>
        /// <returns></returns>
        public T OrElseThrow<Ex>(Func<Ex> exceptionSupplier) where Ex : Exception
        {
            if (this.IsPresent())
            {
                return this.Value;
            }
            else
            {
                throw exceptionSupplier.Invoke();
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            Optional<T> Other = (Optional<T>)obj;

            return this.Value == Other.Value;
        }

        public bool Equals(Optional<T> other)
        {
            if (this.IsPresent() && other.IsPresent())
            {
                return this.Value.Equals(other.Value);
            }
            else
            {
                return this.IsPresent() == other.IsPresent();
            }
        }

        public override int GetHashCode()
        {
            if (this.Value != null)
            {
                return Hash(this.Value);
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override string ToString()
        {
            return this.Value != null ? String.Format("Optional[{0}]", this.Value) : "Optional.empty";
        }

        public static bool operator ==(Optional<T> left, Optional<T> right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (right is null || left is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Optional<T> left, Optional<T> right)
        {
            return !(left == right);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Computes a hash for a set of objects
        /// </summary>
        /// <param name="args">The arguments to hash</param>
        /// <returns>The hash code of the objects</returns>
        private static int Hash(params object[] args)
        {
            unchecked // Overflow is fine, just wrap
            {
                int Hash = 17;

                foreach (object Item in args)
                {
                    if (Item != null)
                    {
                        Hash = (Hash * 23) + Item.GetHashCode();
                    }
                }

                return Hash;
            }
        }

        #endregion
    }
}
